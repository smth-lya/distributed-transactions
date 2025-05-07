using DT.Common.Messaging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<PaymentWorker>();
builder.Services.AddHostedService<PaymentWorker>(sp => sp.GetRequiredService<PaymentWorker>());
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();