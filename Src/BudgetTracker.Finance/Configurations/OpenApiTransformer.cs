using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace BudgetTracker.Finance.Configurations;

public sealed class OpenApiTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes["ApiKey"] = new OpenApiSecurityScheme 
        {
            Type = SecuritySchemeType.ApiKey,
            In = ParameterLocation.Header,
            Name = "API_KEY"
        };

        return Task.CompletedTask;
    }
}