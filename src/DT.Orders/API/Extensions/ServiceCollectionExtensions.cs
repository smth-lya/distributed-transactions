using DT.Orders.API.Consumers.Orchestration;
using DT.Orders.Application.Services;
using DT.Orders.Domain.Contracts.Repositories;
using DT.Orders.Domain.Contracts.Services;
using DT.Orders.Infrastructure.Database;
using DT.Orders.Infrastructure.Database.Repositories;
using DT.Orders.Infrastructure.Decorators;
using DT.Orders.Infrastructure.Messaging;
using DT.Shared.Messaging;
using Microsoft.EntityFrameworkCore;

namespace DT.Orders.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IOrderService, OrderService>();
        return services;
    }
    
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<OrderDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("POSTGRES_CONNECTION")));

        services.AddScoped<IOrderRepository, OrderRepository>();
        services.Decorate<IOrderService, EventPublishingOrderServiceDecorator>();

        services.AddRabbitMq();
        
        return services;
    }

    public static IServiceCollection AddConsumers(this IServiceCollection services)
    {
        services.AddHostedService<OrderCompleteConsumer>();
        
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