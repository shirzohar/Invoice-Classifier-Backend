using BusuMatchProject.Data;
using BusuMatchProject.Models;
using BusuMatchProject.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace BusuMatchProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly JwtService _jwt;

        // Injecting the database context and JWT service
        public AuthController(AppDbContext context, JwtService jwt)
        {
            _context = context;
            _jwt = jwt;
        }

        // ✅ User registration endpoint
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            // Check if a user with this email already exists
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                return BadRequest("User already exists");

            // Hash the password before storing
            var passwordHash = HashPassword(request.Password);

            // Create new user object
            var user = new User
            {
                Email = request.Email,
                PasswordHash = passwordHash
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Generate JWT for the new user
            var token = _jwt.GenerateToken(user);
            return Ok(new { token });
        }

        //  Simple SHA256 hashing for passwords
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        // User login endpoint
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            // Find user by email
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
                return Unauthorized("User not found");

            // Hash the entered password and compare with the stored hash
            var passwordHash = HashPassword(request.Password);
            if (user.PasswordHash != passwordHash)
                return Unauthorized("Invalid password");

            // If credentials match, return JWT token
            var token = _jwt.GenerateToken(user);
            return Ok(new { token });
        }
    }
}
