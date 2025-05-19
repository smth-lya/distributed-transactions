namespace DT.Payments.Application.DTOs;

public record PaymentProcessDto(Guid OrderId, decimal Amount);