using ONIONARCH.Application;
using ONIONARCH.Infrastructure;
using ONIONARCH.Persistence;
using ONIONARCH.Presentation.API.Swagger;

var builder = WebApplication.CreateBuilder(args);

builder.AddApplicationRegistration();
builder.AddPersistenceRegistrations();
builder.AddInfrastructureRegistration();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => SwaggerGenOptionsConfiguration.ApplySwaggerGenOptions(options, builder));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.AddAppSwaggerConfiguration();
}

app.UseHttpsRedirection();
app.Run();