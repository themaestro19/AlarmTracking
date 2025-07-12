using AlarmTracking.Application.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.Application.Entities
{
    public class Role
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }

        protected Role() { }

        public Role(string name, string description = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Role name is required");

            Id = Guid.NewGuid();
            Name = name;
            Description = description ?? string.Empty;
        }
    }
}
