using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BarnameNevis1401.Core;
using BarnameNevis1401.Domains.Users;
using BarnameNevis1401.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace BarnameNevis1401.Controllers;

public class JWTController : Controller
{
    private IUserService _userService;

    private IConfiguration _configuration;
    // GET
    public JWTController(IUserService userService, IConfiguration configuration)
    {
        _userService = userService;
        _configuration = configuration;
    }

    public async Task<IActionResult> Index(LoginModel model)
    {
        var user =  _userService.Login(model.Username, model.Password);
        if (user == null)
            return NotFound();
        var token = MakeJWT(user);

        return Json(new { token });
    }

    private string MakeJWT(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));    
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);    
    
        var claims = new[] {    
            new Claim("UserId", user.Id.ToString()),    
            new Claim(JwtRegisteredClaimNames.Sub, user.Username),    
            new Claim(JwtRegisteredClaimNames.Email, user.Email),    
            new Claim(ClaimTypes.Role, user.IsAdmin?"Admin":"user"),    
            new Claim(JwtRegisteredClaimNames.Jti, user.SerialNumber)    
        };    

        var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],    
            _configuration["Jwt:Issuer"],    
            claims,    
            expires: DateTime.Now.AddMinutes(120),    
            signingCredentials: credentials);    
    
        return new JwtSecurityTokenHandler().WriteToken(token);    
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public IActionResult CheckAccess()
    {
        return Json(new { access=true });
    }
}