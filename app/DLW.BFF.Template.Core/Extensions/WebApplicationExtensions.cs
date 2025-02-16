using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

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
                    // Configure JSON endpoint
                    config.SwaggerEndpoint("/swagger/v1/swagger.json", $"{title} v1");

                    // Add default client id
                    var clientId = app.Configuration.GetValue<string>("AzureAd:ClientId");
                    if (clientId is not null)
                    {
                        config.OAuthClientId(clientId);
                    }

                    // Add default scope
                    var audience = app.Configuration.GetValue<string>("AzureAd:Audience");
                    var scopes = app.Configuration.GetValue<string>("AzureAd:Scopes");
                    if (!string.IsNullOrEmpty(audience) && !string.IsNullOrEmpty(scopes))
                    {
                        config.OAuthScopes($"{audience}/{scopes}");
                    }
                });
            }
        }
    }
}
