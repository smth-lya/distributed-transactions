using DT.Orders.Models;

namespace DT.Orders;

public interface IOrderService
{
    Task CreateOrderAsync();
}