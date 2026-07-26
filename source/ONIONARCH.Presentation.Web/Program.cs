using MudBlazor.Services;
using ONIONARCH.Application;
using ONIONARCH.Infrastructure;
using ONIONARCH.Infrastructure.Exceptions;
using ONIONARCH.Persistence;
using ONIONARCH.Persistence.Providers;
using ONIONARCH.Presentation.Web.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.AddApplicationRegistration();

IDatabaseProvider databaseProvider = new SqlServerDatabaseProvider();
// IDatabaseProvider databaseProvider = new PostgreSqlDatabaseProvider();

builder.AddEFCorePersistenceRegistrations(databaseProvider);
builder.AddDapperPersistenceRegistrations(databaseProvider);
builder.AddInfrastructureRegistration();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddMudServices();

var app = builder.Build();

app.UseExceptionHandler();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();
app.AddInfrastructureApplicationRegistration();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

await app.RunAsync();
