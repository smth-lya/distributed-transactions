using DT.Payments.Outbox.Infrastructure.Database;
using DT.Payments.Outbox.Infrastructure.Messaging;
using DT.Shared.Messaging;
using Microsoft.EntityFrameworkCore;

namespace DT.Payments.Outbox.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("POSTGRES_CONNECTION")));

        services.AddHostedService<OutboxProcessor>();
        
        services.AddRabbitMq();
        
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