using AlarmTracking.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.Application.Entities
{
    public class WorkInstruction : Entity
    {
        public string Id { get; private set; }
        public string ProductCode { get; private set; }
        public string ProcessStep { get; private set; }
        public string Instructions { get; private set; }
        public List<ProcessParameter> Parameters { get; private set; }
        public List<QualityCheck> QualityChecks { get; private set; }
        public TimeSpan EstimatedDuration { get; private set; }

        public WorkInstruction()
        {
            Parameters = new List<ProcessParameter>();
            QualityChecks = new List<QualityCheck>();
        }

        public void AddParameter(string name, string value, string unit)
        {
            Parameters.Add(new ProcessParameter(name, value, unit));
        }

        public void AddQualityCheck(string checkName, string checkType, string criteria, bool isMandatory)
        {
            QualityChecks.Add(new QualityCheck(checkName, checkType, criteria, isMandatory));
        }
    }
}
