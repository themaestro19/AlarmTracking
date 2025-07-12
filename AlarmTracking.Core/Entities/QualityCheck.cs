using AlarmTracking.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.Application.Entities
{
    public class QualityCheck : ValueObject
    {
        public string CheckName { get; private set; }
        public string CheckType { get; private set; }
        public string AcceptanceCriteria { get; private set; }
        public bool IsMandatory { get; private set; }

        public QualityCheck(string checkName, string checkType, string acceptanceCriteria, bool isMandatory)
        {
            CheckName = checkName;
            CheckType = checkType;
            AcceptanceCriteria = acceptanceCriteria;
            IsMandatory = isMandatory;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return CheckName;
            yield return CheckType;
            yield return AcceptanceCriteria;
            yield return IsMandatory;
        }
    }
}
