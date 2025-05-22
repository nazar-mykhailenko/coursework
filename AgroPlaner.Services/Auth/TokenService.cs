using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AgroPlaner.DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AgroPlaner.Services.Auth
{
    public interface ITokenService
    {
        Task<string> GenerateJwtToken(ApplicationUser user);
    }

    public class TokenService : ITokenService
    {
        private readonly JwtConfig _jwtConfig;
        private readonly UserManager<ApplicationUser> _userManager;

        public TokenService(IOptions<JwtConfig> jwtConfig, UserManager<ApplicationUser> userManager)
        {
            _jwtConfig = jwtConfig.Value;
            _userManager = userManager;
        }

        public async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);

            var userRoles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new Claim("UserId", user.Id ?? string.Empty),
                new Claim(ClaimTypes.NameIdentifier, user.Id ?? string.Empty),
            };

            // Add roles to claims
            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtConfig.ExpiryInMinutes),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256
                ),
                Issuer = _jwtConfig.Issuer,
                Audience = _jwtConfig.Audience,
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            return jwtTokenHandler.WriteToken(token);
        }
    }
}
