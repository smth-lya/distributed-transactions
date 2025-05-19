using DT.Orders.Application.Services;
using DT.Orders.Domain.Contracts.Repositories;
using DT.Orders.Domain.Contracts.Services;
using DT.Orders.Infrastructure.Database;
using DT.Orders.Infrastructure.Database.Repositories;
using DT.Orders.Infrastructure.Decorators;
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
}