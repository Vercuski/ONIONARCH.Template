using ONIONARCH.Application;
using ONIONARCH.Infrastructure;
using ONIONARCH.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.AddApplicationRegistration();
builder.AddPersistenceRegistrations();
builder.AddInfrastructureRegistration();

builder.Services.AddEndpointsApiExplorer();
/* Uncomment when using Swagger
builder.Services.AddSwaggerGen(options => SwaggerGenOptionsConfiguration.ApplySwaggerGenOptions(options, builder));
*/

var app = builder.Build();

/* Uncomment when using Swagger
if (!app.Environment.IsProduction())
{
    app.AddAppSwaggerConfiguration();
}
*/

/* Uncomment when using Scalar
if (!app.Environment.IsProduction())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}
*/

app.MapControllers();
app.AddInfrastructureApplicationRegistration();
app.UseHttpsRedirection();
app.Run();