using DT.Payments.Domain.Enums;
using DT.Shared.Enums;

namespace DT.Payments.Domain.Exceptions;

public class InvalidPaymentStatusException : Exception
{
    public InvalidPaymentStatusException(PaymentStatus currentStatus, PaymentStatus? requiredStatus = null)
        : base(requiredStatus.HasValue
            ? $"Invalid payment status: {currentStatus}. Required status: {requiredStatus}"
            : $"Invalid payment status: {currentStatus}")
    {
        CurrentStatus = currentStatus;
        RequiredStatus = requiredStatus;
    }

    public PaymentStatus CurrentStatus { get; }
    public PaymentStatus? RequiredStatus { get; }
}