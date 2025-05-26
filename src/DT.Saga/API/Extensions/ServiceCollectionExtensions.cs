using DT.Saga.API.Consumers.Orchestration.Inventory;
using DT.Saga.API.Consumers.Orchestration.Order;
using DT.Saga.API.Consumers.Orchestration.Payment;
using DT.Saga.Domain.Contracts.Repositories;
using DT.Saga.Infrastructure.Database;
using DT.Saga.Infrastructure.Database.Repositories;
using DT.Saga.Messaging;
using DT.Shared.Messaging;
using Microsoft.EntityFrameworkCore;

namespace DT.Saga.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services;
    }
    
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<SagaDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("POSTGRES_CONNECTION")));

        services.AddScoped<ISagaRepository, SagaRepository>();
        
        services.AddRabbitMq();
        
        return services;
    }

    public static IServiceCollection AddConsumers(this IServiceCollection services)
    {
        services.AddHostedService<InventoryReservationCanceledConsumer>();
        services.AddHostedService<InventoryReservationFailedConsumer>();
        services.AddHostedService<InventoryReservedConsumer>();

        services.AddHostedService<OrderCompletedConsumer>();
        services.AddHostedService<OrderCreatedConsumer>();
        services.AddHostedService<OrderRejectedConsumer>();
        
        services.AddHostedService<PaymentCompletedConsumer>();
        services.AddHostedService<PaymentFailedConsumer>();
        
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