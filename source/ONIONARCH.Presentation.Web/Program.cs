using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using MudBlazor.Services;
using ONIONARCH.Application;
using ONIONARCH.Infrastructure;
using ONIONARCH.Infrastructure.HealthChecks;
using ONIONARCH.Persistence;
using ONIONARCH.Presentation.Web.Components;

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
app.AddInfrastructureApplicationRegistration();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
