using BudgetTracker.Shared.Middlwares;

namespace BudgetTracker.Finance.Extensions;

public static class WebApplicationExtensions
{
    private static void UseSwagger(WebApplication application)
    {
        if (application.Environment.IsDevelopment())
        {
            application.UseSwagger();
            application.UseSwaggerUI();
        }        
    }

    public static WebApplication UseApplicationServices(this WebApplication application)
    {        
        UseSwagger(application);
        application.UseRouting();
        application.UseAuthentication();
        application.UseAuthorization();
        application.MapControllers();
        application.UseHttpsRedirection();
        application.UseMiddleware<ValidateTraceIdMiddleware>();
        application.UseMiddleware<ExceptionHandlerMiddleware>();

        return application;
    }
}