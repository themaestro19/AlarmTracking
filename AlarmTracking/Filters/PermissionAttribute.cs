using AlarmTracking.WebService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace AlarmTracking.WebService.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RequirePermissionAttribute : TypeFilterAttribute
    {
        public RequirePermissionAttribute(string permission) : base(typeof(PermissionFilter))
        {
            Arguments = new object[] { permission };
        }
    }

    public class PermissionFilter : IAuthorizationFilter
    {
        private readonly string _permission;
        private readonly ICurrentUserService _currentUserService;

        public PermissionFilter(string permission, ICurrentUserService currentUserService)
        {
            _permission = permission;
            _currentUserService = currentUserService;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!_currentUserService.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            if (!_currentUserService.HasPermission(_permission))
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
