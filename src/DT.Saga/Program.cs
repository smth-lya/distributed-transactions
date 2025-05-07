using DT.Saga;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<Orchestrator>();
builder.Services.AddHostedService<Orchestrator>(sp => sp.GetRequiredService<Orchestrator>());
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();


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






