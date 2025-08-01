using DT.Saga;
using DT.Saga.API.Extensions;
using DT.Saga.Extensions;
using DT.Saga.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(context.Configuration);
});

builder.Services
    .AddApplication(builder.Configuration)
    .AddInfrastructure(builder.Configuration)
    .AddConsumers(); 

// OpenTelemetry

var resourceBuilder = ResourceBuilder.CreateDefault()
    .AddService(
        serviceName: builder.Environment.ApplicationName,
        serviceVersion: "1.0.0"
    );

builder.Services.AddOpenTelemetry()
    .WithTracing(tracerProviderBuilder =>
    {
        tracerProviderBuilder
            .SetResourceBuilder(resourceBuilder)
            .AddAspNetCoreInstrumentation(options =>
            {
                options.RecordException = true;
                options.EnrichWithHttpRequest = (activity, request) =>
                {
                    activity.SetTag("http.client_ip", request.HttpContext.Connection.RemoteIpAddress);
                };
            })
            .AddEntityFrameworkCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddOtlpExporter();
    })
    .WithMetrics(metricsProviderBuilder =>
    {
        metricsProviderBuilder
            .SetResourceBuilder(resourceBuilder)
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddRuntimeInstrumentation()
            .AddOtlpExporter();
    });

builder.Logging.AddOpenTelemetry(options =>
{
    options.SetResourceBuilder(resourceBuilder)
        .AddOtlpExporter();
});

//

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    MigrateDatabase(app);
}

app.UseHttpsRedirection();
app.UseSerilogRequestLogging();

app.Run();
return;

static void MigrateDatabase(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<SagaDbContext>();

    if (dbContext.Database.GetPendingMigrations().Any())
    {
        dbContext.Database.Migrate();
    }
}

/*
var repo = new InMemoryRepository<OrderState>();
var bus = new RabbitMqMessageBus();

new ISagaStep<OrderState>[]
    {
        new ReserveInventoryStep(bus),
        new ProcessPaymentStep(bus),
        new ShipOrderStep(bus)
    });
*/ 


/*
 * ЧТО НУЖНО СДЕЛАТЬ ДО ПОКАЗА РАБОЧЕЙ ВЕРСИИ 16.04 19:00
 *
 * - Разобраться прям, что такое Outbox pattern, зачем он нужен, как реализовывается, где применяется,
 * с какими продуктами интегрируется
 * - Разобраться что такое BusMessage. Тоже самое ли, что и EventBus... Разобрать реализацию
 * понять нужно ли на моем этапе это иметь
 * - Понять, чем у меня Context.AddEvent отличается от bus.PublishAsync
 * - Понять, нужен ли мне rabbitMq на тек этапе, смогу ли я его протестить.
 * - Реализовать репозитории для саги.
 * И аналогично понять, нужно ли inmemory или redis или postgres
 * - Реализовать нереализованные интерфейсы.
 * - Починить все и добавить больше Steps
 * - Разделить все на разные микросервисы.
 * Разбросать. построить модель архитектуры hexagonal. Понять, что это и почему так называется.
 * Понять, можно ли и правильно ли будет разбить проект на несколько sln с под-sln с проектами.
 * - Добавить OpenTelemetry для трейсинга запросов.
 * - Реализовать тесты для проверок (опционально)
 *
 * Должны ли сервисы внутренние такие как RabbitMQ, Redis, PostgreSql и мои кастомные сервисы иметь
 * публичные открытые порты для доступа? Нужно ли их указывать в docker-compose явно или нет?
 * Или только nginx, ApiGateway должны иметь доступ потключения к др сервисам для переадресации?
 * Или же как-то делается приватная внутренняя сеть?
 */






