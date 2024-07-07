using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using SportEventsAPI.Interface;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace SportEventsAPI.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IConfiguration _configuration;

        public AuthenticationService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetTokenFromHeader(HttpContext context)
        {
            var authorizationHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            return authorizationHeader?.Split(" ").Last();
        }

        public bool ValidateToken(string token, TokenValidationParameters tokenValidationParameters, out SecurityToken validatedToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out validatedToken);
                return principal != null;
            }
            catch (SecurityTokenException ex)
            {
                Console.WriteLine($"Token validation failed: {ex.Message}");
                validatedToken = null;
                return false;
            }
        }

        public IEnumerable<Claim> GetClaimsFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = (JwtSecurityToken)tokenHandler.ReadToken(token);
            return jwtToken.Claims;
        }
    }
}
