using DT.Payments.Domain.Contracts.Repositories;
using DT.Payments.Domain.Models;

namespace DT.Payments.Infrastructure.Database.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private PaymentDbContext _context;
    
    public PaymentRepository(PaymentDbContext context)
    {
        _context = context;
    }
    
    public async Task<Payment?> GetByIdAsync(Guid paymentId)
    {
        return await _context.Payments.FindAsync(paymentId);
    }
    
    public async Task AddAsync(Payment payment)
    {
        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Payment payment)
    {
        _context.Payments.Update(payment);
        await _context.SaveChangesAsync();
    }
}