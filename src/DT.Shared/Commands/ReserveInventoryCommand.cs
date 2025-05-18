using DT.Shared.DTOs;
using DT.Shared.Messaging;

namespace DT.Shared.Commands;

public record ReserveInventoryCommand(List<InventoryItem> Items)
    : IMessage;