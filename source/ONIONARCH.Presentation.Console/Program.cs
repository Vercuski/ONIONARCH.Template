// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ONIONARCH.Application;
using ONIONARCH.Infrastructure;
using ONIONARCH.Persistence;
using ONIONARCH.Presentation.Console;
using Spectre.Console;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
((IHostApplicationBuilder)builder).AddApplicationRegistration();
builder.AddPersistenceRegistrations();
builder.AddInfrastructureRegistration();
builder.Services.AddHostedService<Worker>();

IHost host = builder.Build();
await host.RunAsync();
AnsiConsole.Write(new Markup("[bold red]Hello World![/]"));
AnsiConsole.Write(new Markup("[dim blue]This is dim blue[/]"));