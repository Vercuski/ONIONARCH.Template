// Composition root for the REST API host. Each layer contributes its own registrations; the
// middleware order below matters (see comments).
using ONIONARCH.Application;
using ONIONARCH.Infrastructure;
using ONIONARCH.Infrastructure.Exceptions;
using ONIONARCH.Persistence;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Service registration: MVC controllers, OpenAPI, the global exception handler, then each layer.
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.AddApplicationRegistration();
builder.AddPersistenceRegistrations();
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