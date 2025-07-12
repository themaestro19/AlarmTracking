using AlarmTracking.Application.Contracts.Persistence;
using AlarmTracking.Application.Entities;
using AlarmTracking.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.Application.Services
{
    public class OrderSchedulingDomainService : IOrderSchedulingDomainService
    {
        private readonly IProductionScheduleRepository _scheduleRepository;
        private readonly IBanburyMachineRepository _machineRepository;
        private readonly IProductSpecificationRepository _specRepository;

        public OrderSchedulingDomainService(
            IProductionScheduleRepository scheduleRepository,
            IBanburyMachineRepository machineRepository,
            IProductSpecificationRepository specRepository)
        {
            _scheduleRepository = scheduleRepository;
            _machineRepository = machineRepository;
            _specRepository = specRepository;
        }

        public async Task<SchedulingResult> ScheduleOrderAsync(ProductionOrder order)
        {
            if (!order.CanBeScheduled())
            {
                return SchedulingResult.CreateFailure("Order cannot be scheduled in current status");
            }

            // Get available machines
            var availableMachines = await _machineRepository.GetAvailableMachinesAsync();
            if (!availableMachines.Any())
            {
                return SchedulingResult.CreateFailure("No available machines");
            }

            // Find suitable machine
            var suitableMachine = await FindSuitableMachine(order, availableMachines);
            if (suitableMachine == null)
            {
                return SchedulingResult.CreateFailure("No suitable machine found");
            }

            // Calculate scheduling parameters
            var startTime = await CalculateStartTime(suitableMachine);
            var duration = await CalculateProductionDuration(order);
            var endTime = startTime.Add(duration);

            // Create schedule
            var schedule = ProductionSchedule.Create(
                order.Id,
                suitableMachine.LineId,
                suitableMachine.Id,
                startTime,
                endTime,
                await GetNextSequenceNumber(suitableMachine.Id));

            await _scheduleRepository.AddAsync(schedule);

            return SchedulingResult.CreateSuccess   (schedule);
        }

        public async Task CancelScheduleAsync(string scheduleId)
        {
            var schedule = await _scheduleRepository.GetByIdAsync(scheduleId);
            if (schedule != null)
            {
                schedule.Cancel();
                await _scheduleRepository.UpdateAsync(schedule);
            }
        }

        public async Task<List<BanburyMachine>> GetAvailableMachinesAsync()
        {
            return await _machineRepository.GetAvailableMachinesAsync();
        }

        private async Task<BanburyMachine> FindSuitableMachine(ProductionOrder order, List<BanburyMachine> machines)
        {
            var compatibleMachines = machines.Where(m => m.CanProduceProduct(order.ProductCode)).ToList();

            if (!compatibleMachines.Any())
                return null;

            // Apply scheduling algorithm - select machine with earliest availability
            var bestMachine = compatibleMachines
                .OrderBy(m => GetNextAvailableTime(m))
                .First();

            return bestMachine;
        }

        private async Task<DateTime> CalculateStartTime(BanburyMachine machine)
        {
            var lastSchedule = await _scheduleRepository.GetLastScheduleForMachineAsync(machine.Id);
            return lastSchedule?.ScheduledEndTime.AddHours(1) ?? DateTime.UtcNow.AddHours(2);
        }

        private async Task<TimeSpan> CalculateProductionDuration(ProductionOrder order)
        {
            var spec = await _specRepository.GetByProductCodeAsync(order.ProductCode);
            var baseTime = spec?.BaseCycleTime ?? TimeSpan.FromHours(4);
            var quantityFactor = Math.Ceiling((double)order.Quantity / 100); // Assuming 100 units per batch
            return TimeSpan.FromTicks(baseTime.Ticks * (long)quantityFactor);
        }

        private DateTime GetNextAvailableTime(BanburyMachine machine)
        {
            // Simplified - would query actual schedule
            return DateTime.UtcNow.AddHours(2);
        }

        private async Task<int> GetNextSequenceNumber(string machineId)
        {
            var lastSequence = await _scheduleRepository.GetLastSequenceNumberAsync(machineId);
            return lastSequence + 1;
        }
    }
}
