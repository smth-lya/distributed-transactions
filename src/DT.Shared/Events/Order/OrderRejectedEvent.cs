using DT.Shared.Messaging;

namespace DT.Shared.Events.Order;

public record OrderRejectedEvent(string Reason)
    : IEvent;