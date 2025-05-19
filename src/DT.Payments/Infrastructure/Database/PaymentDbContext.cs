using DT.Payments.Domain.Models;
using DT.Payments.Infrastructure.Database.Configurations;
using Microsoft.EntityFrameworkCore;

namespace DT.Payments.Infrastructure.Database;

public class PaymentDbContext : DbContext
{
    public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options)
    { }
    
    public DbSet<Payment> Payments => Set<Payment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new PaymentConfiguration());
    }
}