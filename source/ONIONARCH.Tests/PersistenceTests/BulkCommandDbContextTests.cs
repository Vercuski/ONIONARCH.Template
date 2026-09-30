using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ONIONARCH.Application;
using ONIONARCH.Application.Abstractions;
using ONIONARCH.Application.Abstractions.Context;
using ONIONARCH.Application.Actions.SampleEntityEFCore.Commands;
using ONIONARCH.Domain.Entities;
using ONIONARCH.Persistence;
using ONIONARCH.Persistence.Contexts;
using ONIONARCH.Persistence.Providers;
//#if (HasDapper)
using System.Data;
//#endif

namespace ONIONARCH.Tests.PersistenceTests;

/// <summary>
/// Tests for the EF Core bulk port (<see cref="IBulkCommandDbContext"/>) and the sample bulk commands,
/// wired exactly as a host wires them (<c>AddApplicationRegistration</c> + <c>AddPersistenceRegistrations</c>)
/// but against an in-memory SQLite database.
/// </summary>
/// <remarks>
/// A test <see cref="IDatabaseProvider"/> points both CQRS sides at SQLite, and the test project references
/// EFCore.BulkExtensions' SQLite adapter, so the entity-list operations run through the real library and
/// the set-based ones through EF Core's real SQL generation, without a database server.
/// </remarks>
[TestFixture]
public class BulkCommandDbContextTests
{
    /// <summary>Keeps the shared in-memory database alive for the duration of a test.</summary>
    private SqliteConnection keepAlive = null!;

    /// <summary>The root service provider built from the host registrations.</summary>
    private ServiceProvider services = null!;

    /// <summary>The scope each test resolves its services from.</summary>
    private AsyncServiceScope scope;

    /// <summary>
    /// Builds a fresh in-memory database and the host's Application and Persistence registrations.
    /// </summary>
    /// <returns>A task representing the asynchronous setup.</returns>
    [SetUp]
    public async Task SetUp()
    {
        string connectionString = $"Data Source=bulk-{Guid.NewGuid():N};Mode=Memory;Cache=Shared";
        keepAlive = new SqliteConnection(connectionString);
        await keepAlive.OpenAsync();

        services = BuildServices(SqliteTestDatabaseProvider.PlatformKey, connectionString,
            providers => providers.Add(new SqliteTestDatabaseProvider()));
        scope = services.CreateAsyncScope();
        await Resolve<CommandDbContext>().Database.EnsureCreatedAsync();
    }

    /// <summary>
    /// Disposes the scope, the container, and the in-memory database.
    /// </summary>
    /// <returns>A task representing the asynchronous teardown.</returns>
    [TearDown]
    public async Task TearDown()
    {
        await scope.DisposeAsync();
        await services.DisposeAsync();
        await keepAlive.DisposeAsync();
    }

    /// <summary>
    /// Verifies that a bulk insert writes every entity and, when asked, reads the generated keys back.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task BulkInsertAsync_Should_InsertEveryEntity_AndRetrieveGeneratedKeys_WhenAsked()
    {
        SampleEntityDefinition[] entities = [Sample("a", 1), Sample("b", 2), Sample("c", 3)];

        await Resolve<IBulkCommandDbContext>().BulkInsertAsync(entities, retrieveGeneratedKeys: true);

        var rows = await Rows();
        Assert.Multiple(() =>
        {
            Assert.That(rows.Select(r => r.SampleString), Is.EquivalentTo(new[] { "a", "b", "c" }));
            Assert.That(entities.Select(e => e.SampleId), Is.Unique.And.All.GreaterThan(0));
            Assert.That(entities.Select(e => e.SampleId), Is.EquivalentTo(rows.Select(r => r.SampleId)));
        });
    }

    /// <summary>
    /// Verifies that an entity-list bulk update writes each entity's values to the row with its key.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task BulkUpdateAsync_Should_WriteEachEntitysValues_ByKey()
    {
        var seeded = await Seed(Sample("a", 1), Sample("b", 2), Sample("c", 3));
        seeded[0].SampleString = "a2";
        seeded[1].SampleInt = 20;

        await Resolve<IBulkCommandDbContext>().BulkUpdateAsync([seeded[0], seeded[1]]);

        var rows = (await Rows()).ToDictionary(r => r.SampleId);
        Assert.Multiple(() =>
        {
            Assert.That(rows[seeded[0].SampleId].SampleString, Is.EqualTo("a2"));
            Assert.That(rows[seeded[1].SampleId].SampleInt, Is.EqualTo(20));
            Assert.That(rows[seeded[2].SampleId].SampleString, Is.EqualTo("c"));
        });
    }

    /// <summary>
    /// Verifies that an entity-list bulk delete removes exactly the rows with the given keys.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task BulkDeleteAsync_Should_DeleteRows_ByKey()
    {
        var seeded = await Seed(Sample("a", 1), Sample("b", 2), Sample("c", 3));

        await Resolve<IBulkCommandDbContext>().BulkDeleteAsync([new SampleEntityDefinition { SampleId = seeded[1].SampleId }]);

        Assert.That((await Rows()).Select(r => r.SampleString), Is.EquivalentTo(new[] { "a", "c" }));
    }

    /// <summary>
    /// Verifies that a bulk upsert inserts entities with an unsaved key as new rows with distinct
    /// database-generated keys (read back when asked), rather than writing the unset key literally.
    /// </summary>
    /// <remarks>
    /// Only the insert half is asserted here: EFCore.BulkExtensions' SQLite adapter leaves identity keys out
    /// of its <c>INSERT ... ON CONFLICT</c>, so on SQLite an existing entity is inserted again rather than
    /// updated. Updating by key is verified against SQL Server and PostgreSQL, whose adapters match on the key.
    /// </remarks>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task BulkUpsertAsync_Should_InsertEntitiesWithUnsavedKeys_WithGeneratedKeys()
    {
        await Seed(Sample("a", 1));
        SampleEntityDefinition[] fresh = [Sample("new", 2), Sample("newer", 3)];

        await Resolve<IBulkCommandDbContext>().BulkUpsertAsync(fresh, retrieveGeneratedKeys: true);

        var rows = await Rows();
        Assert.Multiple(() =>
        {
            Assert.That(rows.Select(r => r.SampleString), Is.EquivalentTo(new[] { "a", "new", "newer" }));
            Assert.That(rows.Select(r => r.SampleId), Is.Unique.And.All.GreaterThan(0));
            Assert.That(fresh.Select(e => e.SampleId), Is.SubsetOf(rows.Select(r => r.SampleId)).And.Unique);
        });
    }

    /// <summary>
    /// Verifies that entity-list operations (including the multi-step upsert) enlist in a transaction begun
    /// through <see cref="IUnitOfWork"/>, so rolling that transaction back discards their rows.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task EntityListOperations_Should_EnlistInTheUnitOfWorkTransaction()
    {
        await using (var transaction = await Resolve<IUnitOfWork>().BeginTransactionAsync())
        {
            await Resolve<IBulkCommandDbContext>().BulkInsertAsync([Sample("a", 1), Sample("b", 2)]);
            await Resolve<IBulkCommandDbContext>().BulkUpsertAsync([Sample("c", 3)]);
            await transaction.RollbackAsync();
        }

        Assert.That(await Rows(), Is.Empty);
    }

    /// <summary>
    /// Verifies that an empty collection is a no-op for every entity-list operation.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task EntityListOperations_Should_DoNothing_WhenThereAreNoEntities()
    {
        var bulk = Resolve<IBulkCommandDbContext>();
        SampleEntityDefinition[] none = [];

        await bulk.BulkInsertAsync(none);
        await bulk.BulkUpdateAsync(none);
        await bulk.BulkDeleteAsync(none);
        await bulk.BulkUpsertAsync(none);

        Assert.That(await Rows(), Is.Empty);
    }

    /// <summary>
    /// Verifies that a set-based update applies both a constant and a row-relative setter, and only to
    /// the rows its predicate matches.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task UpdateWhereAsync_Should_ApplyConstantAndComputedSetters_ToMatchingRowsOnly()
    {
        await Seed(Sample("a", 1), Sample("b", 2), Sample("c", 3));

        int updated = await Resolve<IBulkCommandDbContext>().UpdateWhereAsync<SampleEntityDefinition>(
            e => e.SampleInt >= 2,
            set => set
                .Set(e => e.SampleBoolean, true)
                .Set(e => e.SampleInt, e => e.SampleInt * 10));

        var rows = (await Rows()).ToDictionary(r => r.SampleString!);
        Assert.Multiple(() =>
        {
            Assert.That(updated, Is.EqualTo(2));
            Assert.That((rows["a"].SampleInt, rows["a"].SampleBoolean), Is.EqualTo((1, false)));
            Assert.That((rows["b"].SampleInt, rows["b"].SampleBoolean), Is.EqualTo((20, true)));
            Assert.That((rows["c"].SampleInt, rows["c"].SampleBoolean), Is.EqualTo((30, true)));
        });
    }

    /// <summary>
    /// Verifies that an update declaring no assignments is rejected before any SQL is generated.
    /// </summary>
    [Test]
    public void UpdateWhereAsync_Should_Throw_WhenNoPropertyIsSet()
    {
        var bulk = Resolve<IBulkCommandDbContext>();

        Assert.ThrowsAsync<ArgumentException>(
            () => bulk.UpdateWhereAsync<SampleEntityDefinition>(e => true, _ => { }));
    }

    /// <summary>
    /// Verifies that a set-based delete removes exactly the rows its predicate matches.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task DeleteWhereAsync_Should_DeleteMatchingRowsOnly()
    {
        await Seed(Sample("a", 1), Sample("b", 2), Sample("c", 3));

        int deleted = await Resolve<IBulkCommandDbContext>().DeleteWhereAsync<SampleEntityDefinition>(e => e.SampleInt != 2);

        var remaining = await Rows();
        Assert.Multiple(() =>
        {
            Assert.That(deleted, Is.EqualTo(2));
            Assert.That(remaining.Select(r => r.SampleString), Is.EqualTo(new[] { "b" }));
        });
    }

    /// <summary>
    /// Verifies the sample bulk commands end to end through <see cref="ISender"/>: create (returning keys),
    /// upsert, update by key, and delete by key.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task SampleBulkCommands_Should_CreateUpsertUpdateAndDelete_ThroughTheSender()
    {
        var sender = Resolve<ISender>();

        var created = await sender.Send(new BulkCreateSampleEntityEFCoreRequest([Sample("a", 1), Sample("b", 2), Sample("c", 3)]), CancellationToken.None);
        var ids = created.Value!;
        var upserted = await sender.Send(new BulkUpsertSampleEntityEFCoreRequest([Sample("d", 4), Sample("e", 5)]), CancellationToken.None);
        var updated = await sender.Send(new BulkUpdateSampleEntityEFCoreRequest([ids[0], ids[1]], true, 5), CancellationToken.None);
        var deleted = await sender.Send(new BulkDeleteSampleEntityEFCoreRequest([ids[2]]), CancellationToken.None);

        var rows = await Rows();
        Assert.Multiple(() =>
        {
            Assert.That(ids, Has.Count.EqualTo(3).And.Unique.And.All.GreaterThan(0));
            Assert.That(upserted.Value, Is.EqualTo(2));
            Assert.That(updated.Value, Is.EqualTo(2));
            Assert.That(deleted.Value, Is.EqualTo(1));
            Assert.That(rows.Select(r => (r.SampleString, r.SampleInt, r.SampleBoolean)),
                Is.EquivalentTo(new[] { ("a", 6, true), ("b", 7, true), ("d", 4, false), ("e", 5, false) }));
        });
    }

    /// <summary>
    /// Verifies that each sample bulk command reports an empty request as a validation failure rather
    /// than issuing a statement.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task SampleBulkCommands_Should_ReturnValidationFailure_WhenRequestIsEmpty()
    {
        var sender = Resolve<ISender>();

        var created = await sender.Send(new BulkCreateSampleEntityEFCoreRequest([]), CancellationToken.None);
        Result<int>[] results =
        [
            await sender.Send(new BulkUpsertSampleEntityEFCoreRequest([]), CancellationToken.None),
            await sender.Send(new BulkUpdateSampleEntityEFCoreRequest([], true, 1), CancellationToken.None),
            await sender.Send(new BulkDeleteSampleEntityEFCoreRequest([]), CancellationToken.None)
        ];

        Assert.Multiple(() =>
        {
            Assert.That(created.IsSuccess, Is.False);
            Assert.That(created.ErrorType, Is.EqualTo(ResultErrorType.Validation));
            Assert.That(results, Has.All.Matches<Result<int>>(r => !r.IsSuccess && r.ErrorType == ResultErrorType.Validation));
        });
    }

    /// <summary>
    /// Verifies that on a platform without a bulk adapter the entity-list operations fail fast with a
    /// message naming the platform, before touching the database, while the set-based ones stay available.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task EntityListOperations_Should_ThrowNotSupported_OnAPlatformWithoutABulkAdapter()
    {
        const string platform = "NoBulk";
        string connectionString = keepAlive.ConnectionString;
        await using var noBulkServices = BuildServices(platform, connectionString,
            providers => providers.Add(new SqliteTestDatabaseProvider(platform, supportsBulkOperations: false)));
        await using var noBulkScope = noBulkServices.CreateAsyncScope();
        var bulk = noBulkScope.ServiceProvider.GetRequiredService<IBulkCommandDbContext>();
        SampleEntityDefinition[] entities = [Sample("a", 1)];

        var ex = Assert.ThrowsAsync<NotSupportedException>(() => bulk.BulkInsertAsync(entities));
        Assert.ThrowsAsync<NotSupportedException>(() => bulk.BulkUpdateAsync(entities));
        Assert.ThrowsAsync<NotSupportedException>(() => bulk.BulkDeleteAsync(entities));
        Assert.ThrowsAsync<NotSupportedException>(() => bulk.BulkUpsertAsync(entities));
        int deleted = await bulk.DeleteWhereAsync<SampleEntityDefinition>(e => true);

        Assert.Multiple(() =>
        {
            Assert.That(ex!.Message, Does.Contain(platform));
            Assert.That(deleted, Is.Zero);
        });
    }

    /// <summary>
    /// Builds the service provider exactly as a host does, for one platform on both CQRS sides.
    /// </summary>
    /// <param name="platform">The platform key to configure.</param>
    /// <param name="connectionString">The connection string for both sides.</param>
    /// <param name="configureProviders">Registers the provider for <paramref name="platform"/>.</param>
    /// <returns>The root service provider.</returns>
    private static ServiceProvider BuildServices(string platform, string connectionString, Action<DatabaseProviderRegistry> configureProviders)
    {
        var builder = Host.CreateEmptyApplicationBuilder(new HostApplicationBuilderSettings());
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["DatabasePlatform:QueryDbPlatform"] = platform,
            ["DatabasePlatform:CommandDbPlatform"] = platform,
            ["ConnectionStrings:QueryDbConnection"] = connectionString,
            ["ConnectionStrings:CommandDbConnection"] = connectionString
        });
        builder.Services.AddLogging();
        builder.AddApplicationRegistration();
        builder.AddPersistenceRegistrations(configureProviders);
        return builder.Services.BuildServiceProvider(validateScopes: true);
    }

    /// <summary>
    /// Resolves a service from the test scope.
    /// </summary>
    /// <typeparam name="T">The service type.</typeparam>
    /// <returns>The service instance.</returns>
    private T Resolve<T>() where T : notnull => scope.ServiceProvider.GetRequiredService<T>();

    /// <summary>
    /// Inserts rows through a separate, tracked context so the scoped context under test starts clean.
    /// </summary>
    /// <param name="entities">The rows to insert.</param>
    /// <returns>The inserted entities, with their generated keys, detached from any context.</returns>
    private async Task<SampleEntityDefinition[]> Seed(params SampleEntityDefinition[] entities)
    {
        await using var seedScope = services.CreateAsyncScope();
        var context = seedScope.ServiceProvider.GetRequiredService<CommandDbContext>();
        context.SampleEntity.AddRange(entities);
        await context.SaveChangesAsync();
        return entities;
    }

    /// <summary>
    /// Reads every row through a separate context, so the result reflects the database rather than
    /// any tracked state.
    /// </summary>
    /// <returns>All sample rows.</returns>
    private async Task<List<SampleEntityDefinition>> Rows()
    {
        await using var readScope = services.CreateAsyncScope();
        return await readScope.ServiceProvider.GetRequiredService<QueryDbContext>().SampleEntity.ToListAsync();
    }

    /// <summary>
    /// Creates an unsaved sample entity.
    /// </summary>
    /// <param name="text">The sample string.</param>
    /// <param name="number">The sample integer.</param>
    /// <returns>The new entity.</returns>
    private static SampleEntityDefinition Sample(string text, int number) =>
        new() { SampleString = text, SampleInt = number, SampleDecimal = number / 2m };

    /// <summary>
    /// Test <see cref="IDatabaseProvider"/> for SQLite. EFCore.BulkExtensions finds its SQLite adapter
    /// from the EF Core provider in use, so the platform key can be anything.
    /// </summary>
    /// <param name="platform">The platform key to register under.</param>
    /// <param name="supportsBulkOperations">The value to report for <see cref="IDatabaseProvider.SupportsBulkOperations"/>.</param>
    private sealed class SqliteTestDatabaseProvider(string platform = SqliteTestDatabaseProvider.PlatformKey, bool supportsBulkOperations = true)
        : IDatabaseProvider
    {
        /// <summary>The default platform key used in the test configuration.</summary>
        public const string PlatformKey = "SQLite";

        /// <inheritdoc />
        public string Platform => platform;

        /// <inheritdoc />
        public bool SupportsBulkOperations => supportsBulkOperations;

        /// <inheritdoc />
        public void ConfigureEfCore(DbContextOptionsBuilder optionsBuilder, string connectionString)
        {
            optionsBuilder.UseSqlite(connectionString);
        }
//#if (HasDapper)

        /// <inheritdoc />
        public IDbConnection CreateConnection(string connectionString)
        {
            return new SqliteConnection(connectionString);
        }
//#endif
    }
}
