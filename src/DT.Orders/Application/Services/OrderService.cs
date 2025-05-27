using DT.Orders.Application.DTOs;
using DT.Orders.Domain.Contracts.Repositories;
using DT.Orders.Domain.Contracts.Services;
using DT.Orders.Domain.Enums;
using DT.Orders.Domain.Exceptions;
using DT.Orders.Domain.Models;

namespace DT.Orders.Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _repository;
    private readonly ILogger<OrderService> _logger;
    
    public OrderService(IOrderRepository repository, ILogger<OrderService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Order?> GetOrderByIdAsync(Guid id)
        => await _repository.GetByIdAsync(id);

    public async Task<Guid> CreateOrderAsync(OrderCreateDto createDto)
    {
        ArgumentNullException.ThrowIfNull(createDto, nameof(createDto));
        
        // Лучше сделать вытаскивать названия продуктов из отдельного сервиса или репозитория,
        // здесь "Unknown product name" - пока просто заглушка, которую лучше убрать или заменить
        var orderItems = createDto.Items.Select(i => 
            new OrderItem(
                i.ProductId, 
                "Unknown product name", // TODO: Сделать получение реального имени продукта
                i.Quantity, 
                i.Price
            )).ToList();
        
        var order = new Order(
            createDto.CustomerId,
            createDto.ShippingAddress,
            orderItems
            );
        
        await _repository.AddAsync(order);
        
        return order.Id;
    }

    public async Task UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus, string reason)
    {
        var order = await _repository.GetByIdAsync(orderId);
        if (order == null)
        {
            throw new OrderNotFoundException(orderId);
        }
        
        order.ChangeStatus(newStatus, reason);
        Console.WriteLine(new string('-', 50));
        Console.WriteLine(new string('-', 50));
        Console.WriteLine(new string('-', 50));
        Console.WriteLine(order);
        await _repository.UpdateAsync(order);
        Console.WriteLine(new string('S', 50));
        Console.WriteLine(new string('D', 50));
    }
}