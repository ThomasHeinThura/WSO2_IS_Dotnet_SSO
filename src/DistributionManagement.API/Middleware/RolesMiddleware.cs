using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace DistributionManagement.API.Middleware;

public class RolesMiddleware
{
    private readonly RequestDelegate _next;

    public RolesMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var authHeader = context.Request.Headers.Authorization.FirstOrDefault();
        
        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
        {
            var token = authHeader.Substring("Bearer ".Length).Trim();
            
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                // ✅ Get roles from JWT token
                var roles = jwtToken.Claims
                    .Where(c => c.Type == "roles")
                    .Select(c => c.Value)
                    .ToList();

                if (roles.Any() && context.User?.Identity is ClaimsIdentity identity)
                {
                    // Add roles as ClaimTypes.Role
                    foreach (var role in roles)
                    {
                        identity.AddClaim(new Claim(ClaimTypes.Role, role));
                    }
                }
            }
            catch
            {
                // Token parsing failed, continue normally
            }
        }

        await _next(context);
    }
}
