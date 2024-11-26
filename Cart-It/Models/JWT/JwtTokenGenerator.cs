using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Cart_It.Models.JWT
{
    public static class JwtTokenGenerator
    {
        private const string Key = "KHPK6Ucf/zjvU4qW8/vkuuGLHeIo0l9ACJiTaAPLKbk="; // Use a strong secret key in a production environment
        private const string Issuer = "https://localhost:7256"; //Authentication Server Domain URL
        private const string Audience = "Cart-It-Users"; //Client Application Domain URL

        public static string GenerateToken(int customerId, string email)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(ClaimTypes.Name, email),
            new Claim(ClaimTypes.NameIdentifier, customerId.ToString())
        };

            var tokenDescriptor = new JwtSecurityToken(
                issuer: Issuer,
                audience: Audience,
                claims: claims,
                expires: DateTime.Now.AddHours(1), // Set your token expiration time here
                signingCredentials: credentials
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.WriteToken(tokenDescriptor);
            return token;
        }
    }
}
