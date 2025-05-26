namespace DT.Saga.Domain.Enums;

public enum CommandStatus
{
    Pending,
    Completed,
    Failed,
    Compensated
}