using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;

namespace DLW.BFF.Template.Core.Extensions
{
    public static class WebApplicationExtensions
    {
        public static void UseSwagger(this WebApplication app, string? title)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(config =>
                {
                    // Get the Azure AD options from the configuration
                    var azureAdOptions = app.Configuration.GetSection("AzureAd").Get<MicrosoftIdentityOptions>();
                    ArgumentNullException.ThrowIfNull(azureAdOptions, nameof(azureAdOptions));

                    config.SwaggerEndpoint("/swagger/v1/swagger.json", $"{title} v1");

                    // Add default client id
                    config.OAuthClientId(azureAdOptions.ClientId);

                    // Add default scope
                    var audience = app.Configuration.GetValue<string>("AzureAd:Audience");
                    if (!string.IsNullOrEmpty(audience))
                    {
                        config.OAuthScopes(audience);
                    }
                });
            }
        }
    }
}
