using System.Net.Http.Headers;
using BudgetTracker.Shared.Configurations;
using BudgetTracker.Shared.Constants;
using BudgetTracker.Shared.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Yarp.ReverseProxy.Model;
using Yarp.ReverseProxy.Transforms;
using Yarp.ReverseProxy.Transforms.Builder;
using Google.Apis.Auth.OAuth2;

namespace BudgetTracker.Gateway.Extensions;

public static class StartUpExtensions
{
    extension(TransformBuilderContext transformContext)
    {
        public TransformBuilderContext ConfigJwtAuthentication()
        {
            transformContext.AddRequestTransform(context =>
            {
                RouteModel cluster = context.HttpContext.GetRouteModel();
                string? clusterId = cluster.Config.ClusterId;

                if (string.IsNullOrEmpty(clusterId))
                {
                    return ValueTask.CompletedTask; // TODO: Check how to verify cluster id is valid 
                }
            
                // FIX: Since this service is not getting registerd without IOptions<T>, silent gateway error was thrown.
                JwtConfiguration secrets = context.HttpContext.RequestServices.GetRequiredService<IOptions<JwtConfiguration>>().Value;
                secrets.Audience = clusterId;
                string token = JwtFactory.CreateToken(secrets, scope: SharedConstants.Jwt.Scopes.DOWNSTREAM);
                context.ProxyRequest.Headers.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token);
                return ValueTask.CompletedTask;
            });
            
            return transformContext;
        }

        public TransformBuilderContext ConfigGoogleOAuth()
        {
            transformContext.AddRequestTransform(async context =>
            {
                RouteModel cluster = context.HttpContext.GetRouteModel();
                string? address = cluster.Cluster?.Destinations.Values.FirstOrDefault()?.Model.Config.Address;
                
                if (!string.IsNullOrEmpty(address))
                {
                    string audience = address.TrimEnd('/');
                    GoogleCredential? credential = await GoogleCredential.GetApplicationDefaultAsync();
                    OidcToken token = await credential?.GetOidcTokenAsync(OidcTokenOptions.FromTargetAudience(audience))!;
                    string jwt = await token.GetAccessTokenAsync();
                    context.ProxyRequest.Headers.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, jwt);
                }
            });
            
            return transformContext;
        }
    }
}

