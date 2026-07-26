using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ONIONARCH.Application;
using ONIONARCH.Infrastructure;
using ONIONARCH.Infrastructure.Exceptions;
using ONIONARCH.Persistence;
using ONIONARCH.Persistence.Providers;
using ONIONARCH.Presentation.Console;
using Spectre.Console;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.AddApplicationRegistration();

IDatabaseProvider databaseProvider = new SqlServerDatabaseProvider();
// IDatabaseProvider databaseProvider = new PostgreSqlDatabaseProvider();

builder.AddEFCorePersistenceRegistrations(databaseProvider);
builder.AddDapperPersistenceRegistrations(databaseProvider);
builder.AddInfrastructureRegistration();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddHostedService<Worker>();

IHost host = builder.Build();
await host.RunAsync();
AnsiConsole.Write(new Markup("[bold red]Hello World![/]"));
AnsiConsole.Write(new Markup("[dim blue]This is dim blue[/]"));