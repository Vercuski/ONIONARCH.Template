// Composition root for the REST API host. Each layer contributes its own registrations; the
// middleware order below matters (see comments).
using ONIONARCH.Application;
using ONIONARCH.Infrastructure;
using ONIONARCH.Infrastructure.Exceptions;
using ONIONARCH.Infrastructure.Versioning;
using ONIONARCH.Persistence;
//#if (HasMySql)
using ONIONARCH.Persistence.MySql;
//#endif
//#if (HasPostgreSql)
using ONIONARCH.Persistence.PostgreSql;
//#endif
//#if (HasSqlServer)
using ONIONARCH.Persistence.SqlServer;
//#endif
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Service registration: MVC controllers, OpenAPI, the global exception handler, then each layer.
builder.Services.AddControllers();
// The OpenAPI document's info.version reports the build's semantic version (from git tags via MinVer).
builder.Services.AddOpenApi(options => options.AddDocumentTransformer((document, context, _) =>
{
    document.Info.Version = context.ApplicationServices.GetRequiredService<ApplicationVersion>().SemanticVersion;
    return Task.CompletedTask;
}));
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.AddApplicationRegistration();
// Database providers this host can run against; DatabasePlatform in appsettings.json picks one
// per CQRS side. Remove a line (and its project reference) to drop that platform.
builder.AddPersistenceRegistrations(providers =>
{
//#if (HasSqlServer)
    providers.AddSqlServer();
//#endif
//#if (HasPostgreSql)
    providers.AddPostgreSql();
//#endif
//#if (HasMySql)
//#if (HasEfCore)
    providers.AddMySql(builder.Configuration);
//#else
//~    providers.AddMySql();
//#endif
//#endif
});
builder.AddInfrastructureRegistration();

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// OpenAPI document and Scalar API reference UI are exposed outside Production only.
if (!app.Environment.IsProduction())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

// Correlation ID middleware must run before the exception handler so error responses carry the ID.
app.UseCorrelationIdMiddleware();
app.UseExceptionHandler();
app.MapControllers();
app.AddInfrastructureApplicationRegistration();
app.UseHttpsRedirection();
await app.RunAsync();