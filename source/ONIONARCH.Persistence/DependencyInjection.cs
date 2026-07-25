using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using ONIONARCH.Application.Abstractions;
using ONIONARCH.Application.Abstractions.ConnectionFactory;
using ONIONARCH.Application.Abstractions.Context;
using ONIONARCH.Domain.Options;
using ONIONARCH.Persistence.ConnectionFactory;
using ONIONARCH.Persistence.Contexts;

namespace ONIONARCH.Persistence;

public static class DependencyInjection
{
    public static IHostApplicationBuilder AddPersistenceRegistrations(
        this IHostApplicationBuilder builder,
        Action<DbContextOptionsBuilder, string> configureProvider)
    {
        builder.Services.AddScoped<IDbReadOnlyConnectionFactory, SqlDbReadOnlyConnectionFactory>();
        builder.Services.AddScoped<IDbWriteConnectionFactory, SqlDbWriteConnectionFactory>();

        builder.Services.AddDbContext<CommandDbContext>((sp, options) =>
        {
            var connectionStringOptions = sp.GetRequiredService<IOptions<ConnectionStringOptions>>().Value;
            configureProvider(options, connectionStringOptions.CommandDbConnection);
            options.EnableDetailedErrors().EnableSensitiveDataLogging();
        }, ServiceLifetime.Scoped);

        builder.Services.AddDbContext<QueryDbContext>((sp, options) =>
        {
            var connectionStringOptions = sp.GetRequiredService<IOptions<ConnectionStringOptions>>().Value;
            configureProvider(options, connectionStringOptions.QueryDbConnection);
            options.EnableDetailedErrors().EnableSensitiveDataLogging();
        }, ServiceLifetime.Scoped);

        builder.Services.AddTransient<ICommandDbContext>(sp => sp.GetRequiredService<CommandDbContext>());
        builder.Services.AddTransient<IQueryDbContext>(sp => sp.GetRequiredService<QueryDbContext>());
        builder.Services.AddTransient<IUnitOfWork>(sp => sp.GetRequiredService<CommandDbContext>());

        return builder;
    }
}
