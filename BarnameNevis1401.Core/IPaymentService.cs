using BarnameNevis1401.Domains.Payments;

namespace BarnameNevis1401.Core;

public interface IPaymentService
{
    Task AddNewPaymentAsync(Payment payment);
    Task<List<Payment>> GetPaymentsAsync(int skip, int take, int? UserId, string search);
    Task<int> GetPaymentsCountAsync( int? UserId, string search);
    Task<Payment> GetPaymentAsync(int id);
    Task<Payment> GetPaymentAsync(string code);
}