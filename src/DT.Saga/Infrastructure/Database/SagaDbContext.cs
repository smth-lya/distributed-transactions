using System.Reflection;
using DT.Saga.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace DT.Saga.Infrastructure.Database;

public class SagaDbContext : DbContext
{
    public SagaDbContext(DbContextOptions<SagaDbContext> options) 
        : base(options) { }
    
    public DbSet<SagaState> States => Set<SagaState>();
    public DbSet<SagaEvent> Events => Set<SagaEvent>();
    public DbSet<SagaCommand> Commands => Set<SagaCommand>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}