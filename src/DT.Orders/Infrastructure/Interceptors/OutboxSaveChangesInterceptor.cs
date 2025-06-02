using DT.Orders.Domain.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace DT.Orders.Infrastructure.Interceptors;

public class OutboxSaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly ILogger<OutboxSaveChangesInterceptor> _logger;
    
    public OutboxSaveChangesInterceptor(ILogger<OutboxSaveChangesInterceptor> logger)
    {
        _logger = logger;
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData, 
        int result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
            await ProcessOutboxMessages(eventData.Context, cancellationToken);
        
        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    private async Task ProcessOutboxMessages(DbContext context, CancellationToken cancellationToken)
    {
        // var domainEvents = context.ChangeTracker
        //     .Entries<IAggregateRoot>()
        //     .SelectMany(x => x.Entity.Events)
        //     .ToList();
        //
        // var outboxMessages = domainEvents
        //     .Select(domainEvent => 
        //         OutboxMessage.CreateEvent(domainEvent, _serializer))
        //     .ToList();
        //
        // if (outboxMessages.Count != 0)
        // {
        //     await context.Set<OutboxMessage>().AddRangeAsync(outboxMessages, cancellationToken);
        // }
        //
        // foreach (var entry in context.ChangeTracker.Entries<IAggregateRoot>())
        // {
        //     entry.Entity.ClearDomainEvents();
        // }
    }
}
