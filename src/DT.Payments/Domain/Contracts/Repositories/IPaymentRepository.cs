using DT.Payments.Domain.Models;

namespace DT.Payments.Domain.Contracts.Repositories;

public interface IPaymentRepository
{
    Task AddAsync(Payment payment);
    Task UpdateAsync(Payment payment);
    
    Task<Payment?> GetByIdAsync(Guid paymentId);
}