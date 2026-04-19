using BudgetTracker.Shared.Configurations;
using BudgetTracker.Shared.Models;
using Microsoft.Extensions.DependencyInjection;

namespace BudgetTracker.Shared.Extensions;

public static class SharedServiceExtensions
{
    extension(IServiceCollection collection)
    {
        public IServiceCollection AddJwtConfiguration<T>() where T : SharedSecrets
        {
            collection
                .ConfigureOptions<ConfigureJwtOptions<T>>()
                .AddAuthentication()
                .AddJwtBearer();
            
            return collection;
        }
    }
}