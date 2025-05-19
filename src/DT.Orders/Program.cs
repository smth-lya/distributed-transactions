using DT.Shared.Events;
using DT.Shared.Messaging;
using DT.Orders.API.Extensions;
using DT.Orders.Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(context.Configuration);
});

builder.Services
    .AddApplication(builder.Configuration)
    .AddInfrastructure(builder.Configuration);
builder.Services.AddControllers();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseSerilogRequestLogging();

app.Run();

//Выполнение действий на страте приложения
