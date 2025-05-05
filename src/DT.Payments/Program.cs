using DT.Common.Messaging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<PaymentWorker>();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();