using AlarmTracking.Application.Contracts.Persistence;
using AlarmTracking.Application.Common;
using AlarmTracking.Application.Entities;
using AlarmTracking.Application.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.Application.Services
{
    public class TaskDispatchDomainService : ITaskDispatchDomainService
    {
        private readonly IShopfloorTaskRepository _taskRepository;
        private readonly IOperatorRepository _operatorRepository;

        public TaskDispatchDomainService(
            IShopfloorTaskRepository taskRepository,
            IOperatorRepository operatorRepository)
        {
            _taskRepository = taskRepository;
            _operatorRepository = operatorRepository;
        }

        public async Task<TaskDispatchResult> DispatchTasksAsync(ProductionOrder order, ProductionSchedule schedule,
            List<MaterialReservation> reservations, List<WorkInstruction> instructions)
        {
            var tasks = new List<ShopfloorTask>();

            // Create setup tasks
            var setupTask = ShopfloorTask.Create(
                order.Id,
                schedule.WorkStationId,
                TaskType.Setup,
                $"Setup Banbury machine for Order {order.Id}");
            tasks.Add(setupTask);

            // Create production tasks from instructions
            foreach (var instruction in instructions)
            {
                var productionTask = ShopfloorTask.Create(
                    order.Id,
                    schedule.WorkStationId,
                    TaskType.Production,
                    instruction.Instructions);
                tasks.Add(productionTask);
            }

            // Create quality check task
            var qualityTask = ShopfloorTask.Create(
                order.Id,
                schedule.WorkStationId,
                TaskType.QualityCheck,
                $"Quality inspection for Order {order.Id}");
            tasks.Add(qualityTask);

            // Assign operators
            await AssignOperatorsToTasks(tasks);

            // Save tasks
            foreach (var task in tasks)
            {
                await _taskRepository.AddAsync(task);
            }

            return TaskDispatchResult.CreateSuccess(tasks);
        }

        private async Task AssignOperatorsToTasks(List<Entities.ShopfloorTask> tasks)
        {
            foreach (var task in tasks)
            {
                var availableOperators = await _operatorRepository.GetAvailableOperatorsAsync(task.WorkStationId);
                if (availableOperators.Any())
                {
                    var selectedOperator = availableOperators.First();
                    task.AssignToOperator(selectedOperator.Id);
                }
            }
        }
    }
}
