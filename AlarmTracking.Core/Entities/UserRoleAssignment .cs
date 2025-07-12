using AlarmTracking.Application.Common;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.Application.Entities
{
    public class UserRoleAssignment : BaseAuditableEntity
    {
        public Guid UserId { get; private set; }
        public Guid RoleId { get; private set; }
        public DateTime AssignedAt { get; private set; }

        // Navigation properties for EF Core
        public User User { get; private set; } = null!;
        public Role Role { get; private set; } = null!;

        protected UserRoleAssignment() { }

        public UserRoleAssignment(Guid userId, Guid roleId)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            RoleId = roleId;
            AssignedAt = DateTime.UtcNow;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
