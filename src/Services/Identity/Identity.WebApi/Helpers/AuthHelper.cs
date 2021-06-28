using Identity.Core.Entities;
using Identity.WebApi.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Identity.WebApi.Helpers
{
    public class AuthHelper : IAuthHelper
    {
        private readonly UserManager<User> _userManager;
        private readonly ILogger _logger;

        public AuthHelper(UserManager<User> userManager, ILoggerFactory loggerFactory)
        {
            _userManager = userManager;
            _logger = loggerFactory.CreateLogger("AuthHelper");
        }

        public async Task<string> GenerateJwtToken(User user)
        {
            try
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName)
                };

                var roles = await _userManager.GetRolesAsync(user);
                var audiences = new[]
                 {
                   new Claim("aud", "main"),
                   new Claim("aud", "payment"),
                   new Claim("aud", "order"),
                   new Claim("aud", "notification"),
                   new Claim("aud", "product"),
                 };

                claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
                claims.AddRange(audiences);

                var key = new SymmetricSecurityKey(Encoding.UTF8
                    .GetBytes(ConfigExtension.Instance.GetValue<string>("AppSettings:Token")));

                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.Now.AddHours(1),
                    SigningCredentials = credentials,
                    Issuer = ConfigExtension.Instance.GetValue<string>("ApiUrlList:Identity.WebApi"),
                    IssuedAt = DateTime.Now,
                    NotBefore = DateTime.Now
                };

                var tokenHandler = new JwtSecurityTokenHandler();

                var token = tokenHandler.CreateToken(tokenDescriptor);

                return tokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in GenerateJwtToken method");
                return null;
            }

        }


        public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        public bool VerifyUserPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {

            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

                if (computedHash.Where((t, i) => t != passwordHash[i]).Any())
                {
                    return false;
                }
            }

            return true;
        }
    }
}
