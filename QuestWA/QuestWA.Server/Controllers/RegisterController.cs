using System;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestWA.Server.Data;
using QuestWA.Server.Models;

namespace QuestWA.Server.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class RegisterController : ControllerBase
  {
    private readonly AppDbContext _dbContext;

    public RegisterController(AppDbContext dbContext)
    {
      _dbContext = dbContext;
    }

    [HttpPost]
    public async Task<IActionResult> Register([FromBody] RegistrationRequest request)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      // Check if the email is already registered
      if (await _dbContext.Users.AnyAsync(u => u.Email == request.Email))
      {
        return Conflict("Email is already registered.");
      }

      // Hash the password
      var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

      // Create a new user
      var user = new User
      {
        Username = request.Username,
        Email = request.Email,
        PasswordHash = hashedPassword,
        CreatedAt = DateTime.UtcNow
      };

      _dbContext.Users.Add(user);
      await _dbContext.SaveChangesAsync();

      return Ok("Registration successful.");
    }
  }

  public class RegistrationRequest
  {
    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;
  }
}
