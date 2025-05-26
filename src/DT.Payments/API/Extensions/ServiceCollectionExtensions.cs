using DT.Payments.API.Consumers.Orchestration;
using DT.Payments.Application.Services;
using DT.Payments.Domain.Contracts.Repositories;
using DT.Payments.Domain.Contracts.Services;
using DT.Payments.Infrastructure.Database;
using DT.Payments.Infrastructure.Database.Repositories;
using DT.Payments.Infrastructure.Messaging;
using DT.Shared.Messaging;
using Microsoft.EntityFrameworkCore;

namespace DT.Payments.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IPaymentService, PaymentService>();
        
        return services;
    }
    
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<PaymentDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("POSTGRES_CONNECTION")));

        services.AddScoped<IPaymentRepository, PaymentRepository>();
        
        services.AddRabbitMq();
        
        return services;
    }

    public static IServiceCollection AddConsumers(this IServiceCollection services)
    {
        services.AddHostedService<PaymentProcessConsumer>();
        
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