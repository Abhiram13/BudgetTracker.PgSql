using Microsoft.EntityFrameworkCore;
using BudgetTracker.Finance.Configurations;
using BudgetTracker.Shared.Middlwares;

namespace BudgetTracker.Finance.Extensions;

/// <summary>
/// Provides extension methods for configuring the <see cref="WebApplication"/> request pipeline.
/// </summary>
internal static class WebApplicationExtensions
{
    extension(WebApplication application)
    {
        /// <summary>
        /// Registers application services like swagger, authentication and middlewares.
        /// </summary>
        /// <remarks>
        /// This method configures Swagger (in development), routing, authentication, 
        /// authorization, controller mapping, HTTPS redirection, and custom middlewares 
        /// for Trace ID validation and global exception handling.
        /// </remarks>
        /// <returns>The <see cref="WebApplication"/> instance for method chaining.</returns>
        public WebApplication UseApplicationServices()
        {
            application.UseMiddleware<ExceptionHandlerMiddleware>();
            // application.UseMiddleware<ValidateTraceIdMiddleware>();
            application.InitlizeDbMigrations();
            application.UseSwaggerConfiguration();
            application.UseRouting();
            application.UseAuthentication();
            application.UseAuthorization();
            application.UseHttpsRedirection();
            application.MapControllers();
            // application.MapControllerRoute(name: "MVC", pattern: "{controller=Home}/{action=Index}/{id?}");
            application.MapStaticAssets();
            
            return application;
        }
        
        /// <summary>
        /// Enables Swagger and SwaggerUI middleware if the application is running in a Development environment.
        /// </summary>
        /// <returns>The <see cref="WebApplication"/> instance.</returns>
        private WebApplication UseSwaggerConfiguration()
        {
            if (!application.Environment.IsDevelopment()) return application;
            
            application.UseSwagger();
            application.UseSwaggerUI();

            return application;
        }

        private WebApplication InitlizeDbMigrations()
        {
            using (IServiceScope scope = application.Services.CreateScope())
            {
                ILogger<Program> logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
                try
                {
                    logger.LogInformation("DB Migration is starting...");
                    MigrateDbContext dbContext = scope.ServiceProvider.GetRequiredService<MigrateDbContext>();
                    dbContext.Database.Migrate();
                    logger.LogInformation("DB Migration completed");
                }
                catch (Exception e)
                {
                    logger.LogCritical(e, "Exception at Finance Server DB Migrate Setup - {ErrorMessage}", e.InnerException?.Message ?? e.Message);
                }
            }
            
            return application;
        }
    }
}