using BarnameNevis1401.Core;
using BarnameNevis1401.Data.SqlServer;
using BarnameNevis1401.Domains.Users;
using BarnameNevis1401.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BarnameNevis1401.ApplicationService;



public class UserService:IUserService
{
    private ApplicationDbContext _context;
    public UserService(ApplicationDbContext context)
    {
        _context = context;
    }

    public bool IsUserExists(string username)
    {
        return _context.Users.Any(x => x.Username == username);
    }

    public async Task<bool> IsUserExistsAsync(string username)
    {
        return await _context.Users.AnyAsync(x => x.Username == username);
    }
    
    public async Task<RegisterCheck> IsExists(string username, string email, string mobile)
    {
        return await _context
            .Users
            .Select(x => new RegisterCheck()
            {
                EmailExists = _context.Users.Any(v => v.Email == email),
                MobileExists = _context.Users.Any(v => v.Mobile == mobile),
                UsernameExists = _context.Users.Any(v => v.Username == username),
            })
            .FirstOrDefaultAsync();
        
    }

    public async Task NewUser(User user)
    {
       await _context.Users.AddAsync(user);
        
    }

    public void AddOtpCode(OtpCode otpCode)
    {
        _context.OtpCodes.Add(otpCode);
    }

    public OtpCode GetOtpCode(string code)
    {
        return _context.OtpCodes
            .Include(x=>x.User)
            .FirstOrDefault(x => x.Code == code);
    }

    public User Login(string username, string password)
    {
        return _context
            .Users
            .FirstOrDefault(x => x.Username == username && x.Password == password.Hash() && x.IsActive);
    }

  
}