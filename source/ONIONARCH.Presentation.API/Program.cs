using ONIONARCH.Application;
using ONIONARCH.Infrastructure;
using ONIONARCH.Infrastructure.Exceptions;
using ONIONARCH.Persistence;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.AddApplicationRegistration();
builder.AddEFCorePersistenceRegistrations();
builder.AddDapperPersistenceRegistrations();
builder.AddInfrastructureRegistration();

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

if (!app.Environment.IsProduction())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapControllers();
app.AddInfrastructureApplicationRegistration();
app.UseHttpsRedirection();
await app.RunAsync();