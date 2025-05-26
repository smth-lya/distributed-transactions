using DT.Orders.Application.DTOs;
using DT.Orders.Domain.Contracts.Services;
using DT.Orders.Domain.Contracts.UnitOfWorks;
using DT.Orders.Domain.Enums;
using DT.Orders.Domain.Exceptions;
using DT.Orders.Domain.Models;

namespace DT.Orders.Application.Services;

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<OrderService> _logger;
    
    public OrderService(IUnitOfWork unitOfWork, ILogger<OrderService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Order?> GetOrderByIdAsync(Guid id)
        => await _unitOfWork.Orders.GetByIdAsync(id);

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
        
        await _unitOfWork.Orders.AddAsync(order);
        await _unitOfWork.CommitAsync();
        
        return order.Id;
    }

    public async Task UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus, string reason)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
        if (order == null)
        {
            throw new OrderNotFoundException(orderId);
        }
        
        order.ChangeStatus(newStatus, reason);
        
        await _unitOfWork.Orders.UpdateAsync(order);
        await _unitOfWork.CommitAsync();
    }
}