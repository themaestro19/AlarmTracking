using AlarmTracking.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.Application.Entities
{
    public class MachineCapabilities : ValueObject
    {
        public double MixingCapacity { get; private set; }
        public List<string> CompatibleProducts { get; private set; }
        public double MaxTemperature { get; private set; }
        public double MaxPressure { get; private set; }
        public int MaxRPM { get; private set; }

        public MachineCapabilities(double mixingCapacity, List<string> compatibleProducts,
            double maxTemperature, double maxPressure, int maxRPM)
        {
            MixingCapacity = mixingCapacity;
            CompatibleProducts = compatibleProducts ?? new List<string>();
            MaxTemperature = maxTemperature;
            MaxPressure = maxPressure;
            MaxRPM = maxRPM;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return MixingCapacity;
            yield return MaxTemperature;
            yield return MaxPressure;
            yield return MaxRPM;
            foreach (var product in CompatibleProducts)
            {
                yield return product;
            }
        }
    }

    public abstract class ValueObject
    {
        protected static bool EqualOperator(ValueObject left, ValueObject right)
        {
            if (ReferenceEquals(left, null) ^ ReferenceEquals(right, null))
            {
                return false;
            }
            return ReferenceEquals(left, null) || left.Equals(right);
        }

        protected static bool NotEqualOperator(ValueObject left, ValueObject right)
        {
            return !(EqualOperator(left, right));
        }

        protected abstract IEnumerable<object> GetEqualityComponents();

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != GetType())
            {
                return false;
            }

            var other = (ValueObject)obj;
            return this.GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
        }

        public override int GetHashCode()
        {
            return GetEqualityComponents()
                .Select(x => x != null ? x.GetHashCode() : 0)
                .Aggregate((x, y) => x ^ y);
        }
    }
}
