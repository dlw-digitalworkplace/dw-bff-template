using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.OpenApi.Models;

namespace DLW.BFF.Template.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>Adds Swagger documentation and OAuth2 security definition to the IServiceCollection.</summary>
        /// <param name="services">The IServiceCollection to add the Swagger documentation to.</param>
        /// <param name="configuration">The IConfiguration instance containing the Azure AD options.</param>
        /// <param name="title">The Title for the open api document.</param>
        /// <param name="authorizationUrl">The redirect URL to use for the OAuth login. If none is provided, the default login.microsoft online will be used</param>
        /// <returns>The updated IServiceCollection with Swagger documentation and OAuth2 security definition.</returns>
        /// <remarks>
        ///     This method configures Swagger to generate API documentation and adds an OAuth2 security definition for authentication.
        ///     It retrieves the Azure AD options from the configuration and uses them to set up the OAuth2 security scheme.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown when the Azure AD options are null.</exception>
        public static IServiceCollection AddSwagger(this IServiceCollection services, IConfiguration configuration, string? title, string? authorizationUrl = null)
        {
            services.AddSwaggerGen(options =>
            {
                // Get the Azure AD options from the configuration
                var azureAdOptions = configuration.GetSection("AzureAd").Get<MicrosoftIdentityOptions>();
                ArgumentNullException.ThrowIfNull(azureAdOptions, nameof(azureAdOptions));

                // Add the Swagger document
                options.SwaggerDoc("v1", new OpenApiInfo { Title = title, Version = "v1" });

                // Add the OAuth2 security definition
                var audience = configuration.GetValue<string>("AzureAd:Audience");
                var authUrl = authorizationUrl is not null 
                    ? new Uri(authorizationUrl, UriKind.RelativeOrAbsolute)
                    : new Uri($"{azureAdOptions.Instance}{azureAdOptions.TenantId}/oauth2/v2.0/authorize");

                // If a custom authorization URL is provided, don't use a token URL
                var tokenUrl = authorizationUrl is not null
                    ? null
                    : new Uri($"{azureAdOptions.Instance}{azureAdOptions.TenantId}/oauth2/v2.0/token");

                if (!string.IsNullOrEmpty(audience))
                {
                    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.OAuth2,
                        Flows = new OpenApiOAuthFlows()
                        {
                            Implicit = new OpenApiOAuthFlow()
                            {
                                AuthorizationUrl = authUrl,
                                TokenUrl = tokenUrl,
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

        /// <summary>Adds cookie authentication to the specified IServiceCollection.</summary>
        /// <param name="services">The IServiceCollection to add the authentication to.</param>
        /// <param name="configuration">The IConfiguration instance containing the authentication settings.</param>
        /// <returns>The IServiceCollection with the added authentication services.</returns>
        public static IServiceCollection AddCookieAuthentication(this IServiceCollection services, IConfiguration configuration)
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

        /// <summary>Adds OAuth token authentication to the specified IServiceCollection.</summary>
        /// <param name="services">The IServiceCollection to add the authentication to.</param>
        /// <param name="configuration">The IConfiguration instance containing the authentication settings.</param>
        /// <returns>The IServiceCollection with the added authentication services.</returns>
        public static IServiceCollection AddOAuthTokenAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            // Add OAuth bearer token authentication
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApi(configuration.GetSection("AzureAd"));

            return services;
        }
    }
}
