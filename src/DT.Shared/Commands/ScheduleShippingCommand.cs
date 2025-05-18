using DT.Common.DTOs;
using DT.Common.Messaging;

namespace DT.Common.Commands;

public record ScheduleShippingCommand(Address DeliveryAddress, List<ShippingItem> Items)
    : IMessage;