using DT.Common.Commands;
using DT.Common.Events;
using DT.Common.Messaging;
using DT.Orders;
using DT.Orders.DTOs;
using DT.Orders.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<OrderWorker>();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/order", async (OrderRequest request, OrderWorker worker, ILogger<Program> logger) =>
{
    var order = new Order
    {
        Id = Guid.NewGuid(),
        ProductId = request.ProductId,
        Quantity = request.Quantity,
        TotalPrice = request.TotalPrice
    };
    
    var orderCreatedEvent = new OrderCreatedEvent
    (
        order.Id,
        request.ProductId,
        request.Quantity,
        request.TotalPrice
    )
    {
        CorrelationId = Guid.NewGuid()
    };

    await worker.PublishAsync(orderCreatedEvent, "saga.fanout.evt", string.Empty);
});

app.Run();

//Выполнение действий на страте приложения
