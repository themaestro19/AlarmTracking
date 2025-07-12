using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.Application.Features.Users.RegisterUser
{
    public record RegisterUserRequest(
         string Username,
         string Email,
         string Password,
         string? FirstName = null,
         string? LastName = null,
         string? Department = null);
}
