using DT.Common.Commands;
using DT.Common.Events;
using DT.Common.Messaging;
using DT.Orders;
using DT.Orders.DTOs;
using DT.Orders.Models;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<OrderWorker>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<OrderWorker>());
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/random", async ([FromServices]OrderWorker worker, [FromServices]ILogger<Program> logger) =>
{
    logger.LogInformation("Starting order worker /random");
    
    var request = new OrderRequest(Guid.NewGuid(), 10, 100);
    
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
