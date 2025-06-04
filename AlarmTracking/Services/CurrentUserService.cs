using AlarmTracking.WebService.Services.Interfaces;
using System.Security.Claims;

namespace AlarmTracking.WebService.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid UserId => Guid.Parse(_httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value ?? Guid.Empty.ToString());

        public string Email => _httpContextAccessor.HttpContext?.User?.FindFirst("email")?.Value ?? string.Empty;

        public string Username => _httpContextAccessor.HttpContext?.User?.FindFirst("username")?.Value ?? string.Empty;

        public List<string> Roles => _httpContextAccessor.HttpContext?.User?.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList() ?? new List<string>();

        public List<string> Permissions => _httpContextAccessor.HttpContext?.User?.Claims
            .Where(c => c.Type == "permission")
            .Select(c => c.Value)
            .ToList() ?? new List<string>();

        public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

        public bool HasPermission(string permission)
        {
            return Permissions.Contains(permission);
        }

        public bool IsInRole(string role)
        {
            return Roles.Contains(role);
        }
    }
}
