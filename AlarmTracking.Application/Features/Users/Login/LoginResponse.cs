using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.Application.Features.Users.Login
{
    public record LoginResponse(
        string AccessToken,
        string TokenType,
        int ExpiresIn,
        UserInfo User);

    public record UserInfo(
        Guid Id,
        string Username,
        string Email,
        string? FirstName,
        string? LastName,
        string? Department,
        IEnumerable<string> Roles);
}
