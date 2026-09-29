// Composition root for the console (generic host) application: registers each layer plus the
// sample background Worker, then runs until shutdown is requested (e.g. Ctrl+C).
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ONIONARCH.Application;
using ONIONARCH.Infrastructure;
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
using ONIONARCH.Presentation.Console;
using Spectre.Console;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
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
    providers.AddMySql(builder.Configuration);
//#endif
});
builder.AddInfrastructureRegistration();
builder.Services.AddHostedService<Worker>();

IHost host = builder.Build();
await host.RunAsync();
// Sample Spectre.Console output; runs only after the host has shut down.
AnsiConsole.Write(new Markup("[bold red]Hello World![/]"));
AnsiConsole.Write(new Markup("[dim blue]This is dim blue[/]"));