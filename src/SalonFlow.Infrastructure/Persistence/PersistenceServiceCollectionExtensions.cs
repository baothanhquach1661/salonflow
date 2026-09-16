using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace SalonFlow.Infrastructure.Persistence;

public static class PersistenceServiceCollectionExtensions
{
    public static IServiceCollection AddSalonFlowPersistence(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<SalonFlowDbContext>(options =>
            options.UseSqlite(connectionString));

        return services;
    }
}
