// Composition root for the console (generic host) application: registers each layer plus the
// sample background Worker, then runs until shutdown is requested (e.g. Ctrl+C).
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ONIONARCH.Application;
using ONIONARCH.Infrastructure;
using ONIONARCH.Persistence;
using ONIONARCH.Persistence.MySql;
using ONIONARCH.Persistence.PostgreSql;
using ONIONARCH.Persistence.SqlServer;
using ONIONARCH.Presentation.Console;
using Spectre.Console;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.AddApplicationRegistration();
// Database providers this host can run against; DatabasePlatform in appsettings.json picks one
// per CQRS side. Remove a line (and its project reference) to drop that platform.
builder.AddPersistenceRegistrations(providers =>
{
    providers.AddSqlServer();
    providers.AddPostgreSql();
    providers.AddMySql();
});
builder.AddInfrastructureRegistration();
builder.Services.AddHostedService<Worker>();

IHost host = builder.Build();
await host.RunAsync();
// Sample Spectre.Console output; runs only after the host has shut down.
AnsiConsole.Write(new Markup("[bold red]Hello World![/]"));
AnsiConsole.Write(new Markup("[dim blue]This is dim blue[/]"));