using DT.Shared.DTOs;
using DT.Shared.Messaging;

namespace DT.Shared.Commands;

public record ScheduleShippingCommand(Address DeliveryAddress, List<ShippingItem> Items)
    : IMessage;