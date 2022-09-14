using BarnameNevis1401.Data.SqlServer;
using BarnameNevis1401.Domains.Users;
using BarnameNevis1401.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BarnameNevis1401;

public interface IInitializerService
{
    void Seed();
}

public class InitializeService : IInitializerService
{
    private ApplicationDbContext _context;

    public InitializeService(ApplicationDbContext context)
    {
        _context = context;
        _context.Database.Migrate();
    }

    public void Seed()
    {
        if (!_context.Users.Any(x=>x.IsAdmin))
        {
            var user = new User()
            {
                Email = "admin@barnamenvis.dev",
                Mobile = "09123456789",
                Username = "admin",
                Password = "123456".Hash(),
                FirstName = "admin",
                LastName = "",
                IsActive = true,
                IsAdmin = true
            };
            user.GenerateSerialNumber();
            _context.Users.Add(user);
        }

        _context.SaveChanges();
    }
}