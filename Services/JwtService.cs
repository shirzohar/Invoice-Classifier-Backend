using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BusuMatchProject.Models;
using Microsoft.IdentityModel.Tokens;

namespace BusuMatchProject.Services
{
    public class JwtService
    {
        private readonly string _secret;
        private readonly string _issuer;

        // Inject secret key and issuer from appsettings.json
        public JwtService(IConfiguration config)
        {
            _secret = config["Jwt:Key"] ?? throw new Exception("Missing JWT Key");
            _issuer = config["Jwt:Issuer"] ?? "BusuMatchAPI";
        }

        // Generates a JWT token for the given user
        public string GenerateToken(User user)
        {
            // Define claims to include in the token
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // Unique user ID
                new Claim(ClaimTypes.Email, user.Email),                  // User email
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // Unique token ID
            };

            // Create signing credentials using the secret key
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Create the JWT token with claims, expiration, and signature
            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _issuer,
                claims: claims,
                expires: DateTime.Now.AddDays(7), // Token is valid for 7 days
                signingCredentials: creds);

            // Return the token as a string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
