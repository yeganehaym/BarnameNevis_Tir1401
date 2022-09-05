using BarnameNevis1401.Domains.Users;

namespace BarnameNevis1401.Core;

public interface IUserService
{
    bool IsUserExists(string username);
    Task<bool> IsUserExistsAsync(string username);
    Task<RegisterCheck> IsExists(string username, string email, string mobile);
    Task NewUser(User user);
    void AddOtpCode(OtpCode otpCode);
    OtpCode GetOtpCode(string code);
    User Login(string username, string password);
    Task<User> FindUserAsync(int userId);
}

public interface ITest
{
    void X();
}