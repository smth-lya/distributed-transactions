using DT.Shared.Messaging;
using Microsoft.EntityFrameworkCore;

namespace DT.Saga.Outbox.Infrastructure.Database;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    { }

    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
}