using DT.Orders.Application.DTOs;
using DT.Orders.Domain.Models;

namespace DT.Orders.Domain.Contracts.Services;

public interface IOrderService
{
    Task<Order?> GetOrderByIdAsync(Guid id);
    Task<Order> CreateOrderAsync(OrderCreateDto createDto);
    
    Task ReserveOrderAsync(Guid orderId);
    Task CancelOrderAsync(Guid orderId);
}