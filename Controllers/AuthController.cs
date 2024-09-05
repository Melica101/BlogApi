using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;

    public AuthController(IUnitOfWork unitOfWork, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginModel login)
    {
        var user = _unitOfWork.Users.GetUserByUsername(login.Username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(login.Password, user.PasswordHash))
        {
            return Unauthorized("Invalid credentials.");
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:SecretKey"]);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            Issuer = _configuration["JwtSettings:Issuer"],
            Audience = _configuration["JwtSettings:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        return Ok(new { Token = tokenString });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        if (_unitOfWork.Users.GetUserByUsername(model.Username) != null)
        {
            return BadRequest("Username is already taken.");
        }

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);
        var user = new User { Username = model.Username, PasswordHash = hashedPassword };

        await _unitOfWork.Users.AddUserAsync(user);
        await _unitOfWork.CompleteAsync();

        return Ok("User registered successfully.");
    }
}

// Models for Login and Register
public class LoginModel
{
    public string Username { get; set; }
    public string Password { get; set; }
}

public class RegisterModel
{
    public string Username { get; set; }
    public string Password { get; set; }
}
