using AlarmTracking.Application.Common;
using AlarmTracking.Application.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.Application.Entities
{
    public class BanburyMachine : Entity
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public string LineId { get; private set; }
        public MachineStatus Status { get; private set; }
        public double MixingCapacity { get; private set; }
        public string RecipeType { get; private set; }
        public TimeSpan CycleTime { get; private set; }
        public double Temperature { get; private set; }
        public double Pressure { get; private set; }
        public DateTime LastMaintenanceDate { get; private set; }
        public MachineCapabilities Capabilities { get; private set; } // Add this property

        private BanburyMachine() { } // For EF

        // Factory method for creating new machines
        public static BanburyMachine Create(string name, string lineId, double mixingCapacity,
            string recipeType, TimeSpan cycleTime)
        {
            var capabilities = new MachineCapabilities(
                mixingCapacity,
                new List<string> { recipeType },
                200, // MaxTemperature
                50,  // MaxPressure
                100  // MaxRPM
            );

            return new BanburyMachine
            {
                Id = Guid.NewGuid().ToString(),
                Name = name,
                LineId = lineId,
                Status = MachineStatus.Available,
                MixingCapacity = mixingCapacity,
                RecipeType = recipeType,
                CycleTime = cycleTime,
                Temperature = 0,
                Pressure = 0,
                LastMaintenanceDate = DateTime.UtcNow,
                Capabilities = capabilities
            };
        }

        // Factory method for reconstructing from database
        public static BanburyMachine CreateFromDatabase(string id, string name, string lineId,
            MachineStatus status, double mixingCapacity, string recipeType, TimeSpan cycleTime,
            double temperature, double pressure, DateTime lastMaintenanceDate)
        {
            var capabilities = new MachineCapabilities(
                mixingCapacity,
                new List<string> { recipeType },
                200, // MaxTemperature - you might want to store these in DB
                50,  // MaxPressure
                100  // MaxRPM
            );

            return new BanburyMachine
            {
                Id = id,
                Name = name,
                LineId = lineId,
                Status = status,
                MixingCapacity = mixingCapacity,
                RecipeType = recipeType,
                CycleTime = cycleTime,
                Temperature = temperature,
                Pressure = pressure,
                LastMaintenanceDate = lastMaintenanceDate,
                Capabilities = capabilities
            };
        }

        public bool IsAvailableForScheduling()
        {
            return Status == MachineStatus.Available &&
                   LastMaintenanceDate.AddDays(30) > DateTime.UtcNow;
        }

        public bool CanProduceProduct(string productCode)
        {
            return Capabilities.CompatibleProducts.Contains(productCode) ||
                   !string.IsNullOrEmpty(productCode);
        }

        public void UpdateStatus(MachineStatus newStatus)
        {
            Status = newStatus;
        }

        public void UpdateTemperature(double temperature)
        {
            if (temperature > Capabilities.MaxTemperature)
                throw new InvalidOperationException($"Temperature {temperature} exceeds maximum {Capabilities.MaxTemperature}");

            Temperature = temperature;
        }

        public void UpdatePressure(double pressure)
        {
            if (pressure > Capabilities.MaxPressure)
                throw new InvalidOperationException($"Pressure {pressure} exceeds maximum {Capabilities.MaxPressure}");

            Pressure = pressure;
        }

        public void RecordMaintenance()
        {
            LastMaintenanceDate = DateTime.UtcNow;
            Status = MachineStatus.Available;
        }
    }
}
