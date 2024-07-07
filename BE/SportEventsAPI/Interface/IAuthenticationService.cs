using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace SportEventsAPI.Interface
{
    public interface IAuthenticationService
    {
        string GetTokenFromHeader(HttpContext context);
        bool ValidateToken(string token, TokenValidationParameters tokenValidationParameters, out SecurityToken validatedToken);
        IEnumerable<Claim> GetClaimsFromToken(string token);
    }
}
