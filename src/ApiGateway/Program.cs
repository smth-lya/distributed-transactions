using ApiGateway.States;
using ApiGateway.Steps;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
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


public record CreateOrderCommand(Product Product);
public record ReserveProductCommand();
public record ProcessPaymentCommand();
public record CreateShipmentTicketCommand();

public record Product(Guid Id, string Name, long Price);
public record Order(Guid Id, Guid ProductId, int Quantity, OrderStatus Status);
public record Inventory(Guid ProductId, int Stock);
public record Payment(Guid OrderId, OrderStatus Status, decimal Amount);

public enum OrderStatus { Pending, Completed, Aborted }