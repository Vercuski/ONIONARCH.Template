namespace ONIONARCH.Presentation.API.Swagger;

public static class SwaggerConstants
{
    public const string Version = "v1";
    public const string Title = "ONIONARCH TITLE";
    public const string Description = "ONIONARCH DESCRIPTION";
    public const string ContactName = "ONIONARCH CONTACT";
    public const string ContactEmail = "ONIONARCH CONTACT EMAIL";
    public const string SoftwareLicenseName = "ONIONARCH SOFTWARE LICENSE";
    public const string SwaggerEndpointUrl = "/swagger/v1/swagger.json";
    public const string BearerFormat = "JWT";
    public const string OpenApiSecuritySchemeName = "Authorization";
    public const string OpenApiSecuritySchemeDescription = "ONIONARCH SECURITY SCHEME DEFINITION";

    public readonly static Uri TermsOfService = new("https://www.ONIONARCH.TERMSOFSERVICE.URL.com");
    public readonly static Uri ContactUrl = new("https://www.ONIONARCH.CONTACT.URL.com");
    public readonly static Uri SoftwareLicenseUrl = new("https://www.ONIONARCH.SOFTWARE.LICENSE.URL.com");
}
