using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using SportEventsAPI.Interface;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SportEventsAPI.Helpers
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly TokenValidationParameters _tokenValidationParameters;

        public JwtMiddleware(RequestDelegate next, TokenValidationParameters tokenValidationParameters)
        {
            _next = next;
            _tokenValidationParameters = tokenValidationParameters;
        }

        public async Task InvokeAsync(HttpContext context, IAuthenticationService authenticationService)
        {
            var token = authenticationService.GetTokenFromHeader(context);

            if (token != null)
            {
                try
                {
                    if (authenticationService.ValidateToken(token, _tokenValidationParameters, out SecurityToken validatedToken))
                    {
                        var claims = authenticationService.GetClaimsFromToken(token);
                        var identity = new ClaimsIdentity(claims, "jwt");

                        context.User = new ClaimsPrincipal(identity);
                    }
                }
                catch (SecurityTokenException ex)
                {
                    Console.WriteLine($"Token validation failed: {ex.Message}");
                }
            }

            context.Response.OnStarting(() =>
            {
                //context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                context.Response.Headers.Add("Cache-Control", "no-cache, private");
                context.Response.Headers.Add("Content-Security-Policy", "default-src 'self' http: https: data: blob: 'unsafe-inline'; frame-ancestors 'self';");
                context.Response.Headers.Add("Referrer-Policy", "no-referrer-when-downgrade");
                context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
                context.Response.Headers.Add("Server", "Grasfam Web Engine");
                return Task.CompletedTask;
            });


            await _next(context);
        }
    }
}
