using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.Application.Common
{
    public abstract class Entity
    {
        public override bool Equals(object obj)
        {
            return obj is Entity entity && GetType() == entity.GetType();
        }

        public override int GetHashCode()
        {
            return GetType().GetHashCode();
        }
    }
}
