using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ONIONARCH.Presentation.API.Swagger;

public static class SwaggerGenOptionsConfiguration
{
    public static void ApplySwaggerGenOptions(SwaggerGenOptions genOptions, IHostApplicationBuilder builder)
    {
        genOptions.SwaggerDoc(SwaggerConstants.Version, new OpenApiInfo
        {
            Version = SwaggerConstants.Version,
            Title = SwaggerConstants.Title,
            Description = SwaggerConstants.Description,
            TermsOfService = SwaggerConstants.TermsOfService,
            Contact = new OpenApiContact
            {
                Name = SwaggerConstants.ContactName,
                Url = SwaggerConstants.ContactUrl,
                Email = SwaggerConstants.ContactEmail
            },
            License = new OpenApiLicense
            {
                Name = SwaggerConstants.SoftwareLicenseName,
                Url = SwaggerConstants.SoftwareLicenseUrl
            },
        });
        //genOptions.SchemaFilter<SwaggerExcludeFilter>();
        if (builder.Environment.IsProduction())
        {
            genOptions.AddServer(new OpenApiServer
            {
                Url = "https://www.ONIONARCH.PRODUCTION.URL.com/"
            });
        }
        genOptions.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = SwaggerConstants.OpenApiSecuritySchemeDescription,
            Name = SwaggerConstants.OpenApiSecuritySchemeName,
            Type = SecuritySchemeType.Http,
            BearerFormat = SwaggerConstants.BearerFormat,
            Scheme = JwtBearerDefaults.AuthenticationScheme
        });
        genOptions.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type=ReferenceType.SecurityScheme,
                        Id=JwtBearerDefaults.AuthenticationScheme
                    }
                },
                Array.Empty<string>()
            }
        });
    }
}
