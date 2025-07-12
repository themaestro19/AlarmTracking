using AlarmTracking.Application.Contracts.Persistence;
using AlarmTracking.Application.Models;
using AlarmTracking.Application.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.Application.Services
{
    public class WorkInstructionDomainService : IWorkInstructionDomainService
    {
        private readonly IWorkInstructionRepository _instructionRepository;
        private readonly IProductSpecificationRepository _specRepository;

        public WorkInstructionDomainService(
            IWorkInstructionRepository instructionRepository,
            IProductSpecificationRepository specRepository)
        {
            _instructionRepository = instructionRepository;
            _specRepository = specRepository;
        }

        public async Task<WorkInstructionResult> LoadWorkInstructionsAsync(ProductionOrder order, ProductionSchedule schedule)
        {
            var baseInstructions = await _instructionRepository.GetByProductCodeAsync(order.ProductCode);
            if (!baseInstructions.Any())
            {
                return WorkInstructionResult.CreateFailure("No work instructions found");
            }

            var customizedInstructions = await CustomizeInstructions(baseInstructions, order);

            // Validate instructions
            if (!ValidateInstructions(customizedInstructions))
            {
                return WorkInstructionResult.CreateFailure("Invalid work instructions");
            }

            return WorkInstructionResult.CreateSuccess(customizedInstructions);
        }

        public async Task ClearWorkInstructionsAsync(string workStationId)
        {
            // Implementation to clear instructions from workstation
            await Task.CompletedTask;
        }

        private async Task<List<WorkInstruction>> CustomizeInstructions(
            List<WorkInstruction> baseInstructions,
            ProductionOrder order)
        {
            var spec = await _specRepository.GetByProductCodeAsync(order.ProductCode);
            var customized = new List<WorkInstruction>();

            foreach (var instruction in baseInstructions)
            {
                var customInstruction = CloneInstruction(instruction);

                // Customize parameters based on tire specifications
                CustomizeParameters(customInstruction, spec, order);

                customized.Add(customInstruction);
            }

            return customized;
        }

        private void CustomizeParameters(WorkInstruction instruction, ProductSpecification spec, ProductionOrder order)
        {
            // Add tire-specific parameters
            instruction.AddParameter("MixingTime", spec.BaseCycleTime.TotalMinutes.ToString(), "minutes");
            instruction.AddParameter("Temperature", spec.OptimalTemperature.ToString(), "°C");
            instruction.AddParameter("Pressure", spec.OptimalPressure.ToString(), "bar");
            instruction.AddParameter("BatchSize", CalculateBatchSize(order.Quantity).ToString(), "units");
        }

        private bool ValidateInstructions(List<WorkInstruction> instructions)
        {
            return instructions.All(i => !string.IsNullOrEmpty(i.Instructions) && i.Parameters.Any());
        }

        private WorkInstruction CloneInstruction(WorkInstruction original)
        {
            // Implementation to clone work instruction
            return new WorkInstruction
            {
                // Copy properties
            };
        }

        private int CalculateBatchSize(int totalQuantity)
        {
            return Math.Min(totalQuantity, 100); // Max 100 units per batch
        }
    }
}
