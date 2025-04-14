using ApiGateway.States;
using ApiGateway.Steps;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

var repo = new InMemoryRepository<OrderState>();
var bus = new RabbitMqMessageBus();

new ISagaStep<OrderState>[]
    {
        new ReserveInventoryStep(bus),
        new ProcessPaymentStep(bus),
        new ShipOrderStep(bus)
    });
    