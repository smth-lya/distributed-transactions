using DT.Inventories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<InventoryWorker>();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();
