using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;
using WebApplication10.Data;

namespace WebApplication10.Middlewares
{
    public class TokenValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TokenValidationMiddleware> _logger;

        public TokenValidationMiddleware(RequestDelegate next, IServiceProvider serviceProvider, ILogger<TokenValidationMiddleware> logger)
        {
            _next = next;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Headers.ContainsKey("Authorization"))
            {
                var tokenHeader = context.Request.Headers["Authorization"].ToString();
                if (!tokenHeader.StartsWith("Bearer "))
                {
                    _logger.LogWarning("Invalid Authorization header format.");
                    await WriteErrorResponse(context, StatusCodes.Status401Unauthorized, "Authorization header format is incorrect.");
                    return;
                }

                var token = tokenHeader.Split(" ").Last();
                var handler = new JwtSecurityTokenHandler();

                if (handler.CanReadToken(token))
                {
                    var jwtToken = handler.ReadJwtToken(token);

                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                        var storedToken = await dbContext.StoredTokens
                            .FirstOrDefaultAsync(t => t.Token == token && t.Expiration > DateTime.UtcNow);

                        if (storedToken == null)
                        {
                            _logger.LogWarning("Token expired or not found: {Token}", token);
                            await WriteErrorResponse(context, StatusCodes.Status401Unauthorized, "Token is expired or invalid.");
                            return;
                        }

                        var userClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
                        if (storedToken.UserName != userClaim)
                        {
                            _logger.LogWarning("Token does not match the user: {UserName}", userClaim);
                            await WriteErrorResponse(context, StatusCodes.Status401Unauthorized, "Token does not match the user.");
                            return;
                        }

                        // Add user identity to HttpContext for further usage.
                        context.Items["User"] = storedToken.UserName;
                    }
                }
                else
                {
                    _logger.LogWarning("Cannot read token: {Token}", token);
                    await WriteErrorResponse(context, StatusCodes.Status401Unauthorized, "Invalid token format.");
                    return;
                }
            }
            else
            {
                _logger.LogWarning("Authorization header is missing.");
                await WriteErrorResponse(context, StatusCodes.Status401Unauthorized, "Authorization header is missing.");
                return;
            }

            await _next(context);
        }

        private async Task WriteErrorResponse(HttpContext context, int statusCode, string message)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync($"{{ \"error\": \"{message}\" }}");
        }
    }
}
