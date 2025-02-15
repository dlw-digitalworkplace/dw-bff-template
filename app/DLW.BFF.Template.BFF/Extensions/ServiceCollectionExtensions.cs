using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.OpenApi.Models;

namespace DLW.BFF.Template.BFF.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>Adds Swagger documentation and OAuth2 security definition to the IServiceCollection.</summary>
        /// <param name="services">The IServiceCollection to add the Swagger documentation to.</param>
        /// <param name="configuration">The IConfiguration instance containing the Azure AD options.</param>
        /// <returns>The updated IServiceCollection with Swagger documentation and OAuth2 security definition.</returns>
        /// <remarks>
        ///     This method configures Swagger to generate API documentation and adds an OAuth2 security definition for authentication.
        ///     It retrieves the Azure AD options from the configuration and uses them to set up the OAuth2 security scheme.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown when the Azure AD options are null.</exception>
        public static IServiceCollection AddSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(options =>
            {
                // Get the Azure AD options from the configuration
                var azureAdOptions = configuration.GetSection("AzureAd").Get<MicrosoftIdentityOptions>();
                ArgumentNullException.ThrowIfNull(azureAdOptions, nameof(azureAdOptions));

                // Add the Swagger document
                options.SwaggerDoc("v1", new OpenApiInfo { Title = typeof(Program).Assembly.GetName().Name, Version = "v1" });

                // Add the OAuth2 security definition
                var audience = configuration.GetValue<string>("AzureAd:Audience");
                if (!string.IsNullOrEmpty(audience))
                {
                    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.OAuth2,
                        Flows = new OpenApiOAuthFlows()
                        {
                            Implicit = new OpenApiOAuthFlow()
                            {
                                AuthorizationUrl = new Uri($"/microsoftidentity/account/signin", UriKind.Relative),
                                Scopes = new Dictionary<string, string>
                                {
                                    [audience] = "OAuth"
                                }
                            }
                        }
                    });
                }                

                var oauth2SecurityScheme = new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
                };

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    [oauth2SecurityScheme] = [audience]
                });
            });
            return services;
        }

        /// <summary>Adds authentication services and configures cookie properties for ASP.NET Core cookie authentication.</summary>
        /// <param name="services">The IServiceCollection to add the authentication services to.</param>
        /// <param name="configuration">The IConfiguration instance containing the authentication options.</param>
        /// <returns>The updated IServiceCollection with authentication services and configured cookie properties.</returns>
        /// <remarks>
        ///     This method adds Microsoft Identity UI services, configures authentication using Microsoft Identity Web App, and sets up cookie properties for ASP.NET Core cookie authentication.
        /// </remarks>
        public static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            // Add the MS Identity UI services
            services.AddRazorPages().AddMicrosoftIdentityUI();
            services.AddMicrosoftIdentityWebAppAuthentication(configuration);

            // Configure cookie properties for ASP.NET Core cookie authentication.
            services.Configure<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme, options => {
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            return services;
        }
    }
}
