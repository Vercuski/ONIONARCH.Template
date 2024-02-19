namespace ONIONARCH.Presentation.API.Swagger
{
    public static class AppSwaggerConfiguration
    {
        public static WebApplication AddAppSwaggerConfiguration(this WebApplication application)
        {
            application.UseSwagger();
            application.UseSwaggerUI();
            return application;
        }
    }
}
