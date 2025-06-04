namespace AlarmTracking.WebService.Services.Interfaces
{
    public interface ICurrentUserService
    {
        Guid UserId { get; }
        string Email { get; }
        string Username { get; }
        List<string> Roles { get; }
        List<string> Permissions { get; }
        bool IsAuthenticated { get; }
        bool HasPermission(string permission);
        bool IsInRole(string role);
    }
}
