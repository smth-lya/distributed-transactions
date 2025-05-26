using DT.Orders.Application.DTOs;
using DT.Orders.Domain.Enums;
using DT.Orders.Domain.Models;

namespace DT.Orders.Domain.Contracts.Services;

public interface IOrderService
{
    Task<Order?> GetOrderByIdAsync(Guid id);
    Task<Guid> CreateOrderAsync(OrderCreateDto createDto);
    Task UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus, string reason);
}