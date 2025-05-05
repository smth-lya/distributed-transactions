using DT.Orders.Models;
using Microsoft.EntityFrameworkCore;

namespace DT.Orders;

public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
    { }

    public DbSet<Order> Orders => Set<Order>();
}