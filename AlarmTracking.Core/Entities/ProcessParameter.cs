using AlarmTracking.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.Application.Entities
{
    public class ProcessParameter : ValueObject
    {
        public string Name { get; private set; }
        public string Value { get; private set; }
        public string Unit { get; private set; }
        public double MinValue { get; private set; }
        public double MaxValue { get; private set; }

        public ProcessParameter(string name, string value, string unit, double minValue = 0, double maxValue = double.MaxValue)
        {
            Name = name;
            Value = value;
            Unit = unit;
            MinValue = minValue;
            MaxValue = maxValue;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Name;
            yield return Value;
            yield return Unit;
        }
    }
}
