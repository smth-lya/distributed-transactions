using DT.Shared.DTOs;
using DT.Shared.Messaging;

namespace DT.Shared.Commands.Inventory;

public record InventoryReserveCommand(Guid OrderId, List<InventoryItemShared> Items)
    : ICommand;