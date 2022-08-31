using BarnameNevis1401.Core;
using BarnameNevis1401.Data.SqlServer;
using BarnameNevis1401.Domains.Payments;
using Microsoft.EntityFrameworkCore;

namespace BarnameNevis1401.ApplicationService;

public class PaymentService:IPaymentService
{
    private ApplicationDbContext _context;

    public PaymentService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddNewPaymentAsync(Payment payment)
    {
        await _context.Payments.AddAsync(payment);
    }

    private IQueryable<Payment> GetPaymentQuery(int? UserId, string search)
    {
        var query = _context.Payments.AsQueryable();
        if (UserId.HasValue)
        {
            query = query.Where(x => x.UserId == UserId);
        }

        if (String.IsNullOrEmpty(search) == false)
        {
            query = query
                .Where(x => x.RefCode == search
                            || (x.User.FirstName + " " + x.User.LastName).Contains(search)
                            || x.User.Username.Contains(search)
                            || x.User.Email.Contains(search));
        }

        return query;
    }
    public async Task<List<Payment>> GetPaymentsAsync(int skip, int take, int? UserId, string search)
    {
        var query = GetPaymentQuery(UserId, search);
        
        return await query
            .OrderByDescending(x => x.PaymentTime)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<int> GetPaymentsCountAsync(int? UserId, string search)
    {
        var query = GetPaymentQuery(UserId, search);
        return await query.CountAsync();
    }

    public async Task<Payment> GetPaymentAsync(int id)
    {
        return await _context.Payments.FindAsync(id);
    }

    public async Task<Payment> GetPaymentAsync(string code)
    {
        return await _context.Payments.FirstOrDefaultAsync(x => x.RefCode == code);
    }
}