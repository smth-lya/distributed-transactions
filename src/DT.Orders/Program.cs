using DT.Common.Events;
using DT.Common.Messaging;
using DT.Orders;
using DT.Orders.DTOs;
using DT.Orders.Messaging;
using DT.Orders.Models;
using Microsoft.AspNetCore.Mvc;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(context.Configuration);
});

builder.Services.AddRazorPages();

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseSerilogRequestLogging();

app.MapGet("/random", async ([FromServices]IMessagePublisher publisher, [FromServices]ILogger<Program> logger) =>
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

    var message = new OrderCreatedEvent
    (
        order.Id,
        request.ProductId,
        request.Quantity,
        request.TotalPrice
    );

    await publisher.PublishAsync(message, "saga.orchestration.events",  string.Empty,Guid.NewGuid().ToString());
});

app.Run();

//Выполнение действий на страте приложения
