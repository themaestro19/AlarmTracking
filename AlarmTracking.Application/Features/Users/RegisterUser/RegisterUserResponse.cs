using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.Application.Features.Users.RegisterUser
{
    public record RegisterUserResponse(
        Guid Id,
        string Username,
        string Email,
        string? FirstName,
        string? LastName,
        string? Department,
        DateTime CreatedAt);
}
