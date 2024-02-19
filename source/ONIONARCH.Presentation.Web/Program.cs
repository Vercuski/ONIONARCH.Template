using ONIONARCH.Application;
using ONIONARCH.Infrastructure;
using ONIONARCH.Persistence;
using ONIONARCH.Presentation.Web.Components;
using MudBlazor.Services;
using MudBlazor;

var builder = WebApplication.CreateBuilder(args);

builder.AddApplicationRegistration();
builder.AddPersistenceRegistrations();
builder.AddInfrastructureRegistration();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddMudServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();
app.MapHealthChecks("/health");

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
