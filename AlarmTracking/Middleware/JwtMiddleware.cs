using AlarmTracking.Application.Contracts.Infrastructure;

namespace AlarmTracking.WebService.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<JwtMiddleware> _logger;

        public JwtMiddleware(RequestDelegate next, ILogger<JwtMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IJwtService jwtService)
        {
            var token = context.Request.Headers["Authorization"]
                .FirstOrDefault()?.Split(" ").Last();

            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    var userId = jwtService.GetUserIdFromToken(token);
                    if (userId.HasValue)
                    {
                        // Add user ID to context for easy access
                        context.Items["UserId"] = userId.Value;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Invalid JWT token");
                }
            }

            await _next(context);
        }
    }

    public static class JwtMiddlewareExtensions
    {
        public static IApplicationBuilder UseJwtMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<JwtMiddleware>();
        }
    }
}
