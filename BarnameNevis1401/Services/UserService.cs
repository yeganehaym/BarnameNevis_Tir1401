﻿using BarnameNevis1401.Data;
using BarnameNevis1401.Data.Entities;
using BarnameNevis1401.Data.Selectmodels;
using Microsoft.EntityFrameworkCore;

namespace BarnameNevis1401.Services;

public class UserService
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

    public RegisterCheck IsExists(string username, string email, string mobile)
    {
        return _context
            .Users
            .Select(x => new RegisterCheck()
            {
                EmailExists = _context.Users.Any(v => v.Email == email),
                MobileExists = _context.Users.Any(v => v.Mobile == mobile),
                UsernameExists = _context.Users.Any(v => v.Username == username),
            })
            .FirstOrDefault();
        
    }

    public void NewUser(User user)
    {
        _context.Users.Add(user);
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