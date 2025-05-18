using DT.Common.DTOs;
using DT.Common.Messaging;

namespace DT.Common.Commands;

public record ReserveInventoryCommand(List<InventoryItem> Items)
    : IMessage;