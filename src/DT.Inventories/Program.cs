using DT.Inventories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<InventoryWorker>();
builder.Services.AddHostedService<InventoryWorker>(sp => sp.GetRequiredService<InventoryWorker>());
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();
