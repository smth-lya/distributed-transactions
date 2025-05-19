using DT.Payments.Application.Services;
using DT.Payments.Domain.Contracts.Repositories;
using DT.Payments.Domain.Contracts.Services;
using DT.Payments.Infrastructure.Database;
using DT.Payments.Infrastructure.Database.Repositories;
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
}