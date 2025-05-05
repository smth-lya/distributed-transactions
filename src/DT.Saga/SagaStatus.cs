namespace DT.Saga;

public enum SagaStatus 
{ 
    Pending,
    InProgress,
    Completed,
    Compensated,
    Failed
}