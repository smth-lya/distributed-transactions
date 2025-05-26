using DT.Inventories.API.Consumers.Orchestration;
using DT.Inventories.Application.Services;
using DT.Inventories.Domain.Contracts.Repositories;
using DT.Inventories.Domain.Contracts.Services;
using DT.Inventories.Infrastructure.Database;
using DT.Inventories.Infrastructure.Database.Repositories;
using DT.Inventories.Messaging;
using DT.Shared.Messaging;
using Microsoft.EntityFrameworkCore;

namespace DT.Inventories.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IInventoryService, InventoryService>();
        
        return services;
    }
    
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<InventoryDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("POSTGRES_CONNECTION")));

        services.AddScoped<IInventoryRepository, InventoryRepository>();
        
        services.AddRabbitMq();
        
        return services;
    }

    public static IServiceCollection AddConsumers(this IServiceCollection services)
    {
        services.AddHostedService<InventoryReserveConsumer>();   
        
        return services;
    }
    
    private static void AddRabbitMq(this IServiceCollection services)
    {
        services.AddSingleton<RabbitMqBroker>();
        services.AddSingleton<IMessagePublisher>(sp => sp.GetRequiredService<RabbitMqBroker>());
        services.AddSingleton<IMessageSubscriber>(sp => sp.GetRequiredService<RabbitMqBroker>());
        services.AddHostedService(sp => sp.GetRequiredService<RabbitMqBroker>());
    }
}