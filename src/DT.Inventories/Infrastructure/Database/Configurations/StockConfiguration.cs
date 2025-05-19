using DT.Inventories.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DT.Inventories.Infrastructure.Database.Configurations;

public class StockConfiguration : IEntityTypeConfiguration<Stock>
{
    public void Configure(EntityTypeBuilder<Stock> builder)
    {
        builder.ToTable("Stocks");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedNever();
        builder.Property(s => s.ProductId).IsRequired();
        builder.Property(s => s.Quantity).IsRequired();
        builder.Property(s => s.Reserved).IsRequired();
    }
}