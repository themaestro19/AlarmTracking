using AlarmTracking.Application.DTOs;
using AlarmTracking.Application.Entities;
using AlarmTracking.Application.Enums;
using AlarmTracking.Application.Repositories;
using AlarmTracking.WebService.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.Application.Services
{
    public class ProductionOrderService : IProductionOrderApplicationService
    {
        private readonly IOrderSchedulingDomainService _schedulingService;
        private readonly IMaterialReservationDomainService _materialService;
        private readonly IWorkInstructionDomainService _instructionService;
        private readonly ITaskDispatchDomainService _taskService;
        private readonly IProductionOrderRepository _orderRepository;
        public ProductionOrderService() { }

        public async Task<ProcessOrderResponseDto> ProcessOrderAsync(ProcessOrderRequestDto request)
        {
            try
            {
                // Convert DTO to domain entity
                var order = ProductionOrder.Create(
                    request.OrderId,
                    request.ProductCode,
                    request.TireSize,
                    request.TirePattern,
                    request.Quantity,
                    request.RequiredDate,
                    request.Priority,
                    request.CustomerCode);

                // Step 1: Schedule Order
                var scheduleResult = await _schedulingService.ScheduleOrderAsync(order);
                if (!scheduleResult.IsSuccess)
                {
                    return new ProcessOrderResponseDto
                    {
                        Success = false,
                        Message = scheduleResult.ErrorMessage,
                        OrderId = request.OrderId
                    };
                }

                // Step 2: Reserve Materials
                var materialResult = await _materialService.ReserveMaterialsAsync(order, scheduleResult.Schedule);
                if (!materialResult.IsSuccess)
                {
                    await _schedulingService.CancelScheduleAsync(scheduleResult.Schedule.Id);
                    return new ProcessOrderResponseDto
                    {
                        Success = false,
                        Message = materialResult.ErrorMessage,
                        OrderId = request.OrderId
                    };
                }

                // Step 3: Load Work Instructions
                var instructionResult = await _instructionService.LoadWorkInstructionsAsync(order, scheduleResult.Schedule);
                if (!instructionResult.IsSuccess)
                {
                    await _materialService.ReleaseReservationsAsync(materialResult.Reservations);
                    await _schedulingService.CancelScheduleAsync(scheduleResult.Schedule.Id);
                    return new ProcessOrderResponseDto
                    {
                        Success = false,
                        Message = instructionResult.ErrorMessage,
                        OrderId = request.OrderId
                    };
                }

                // Step 4: Dispatch Tasks
                var taskResult = await _taskService.DispatchTasksAsync(order, scheduleResult.Schedule, materialResult.Reservations, instructionResult.Instructions);
                if (!taskResult.IsSuccess)
                {
                    await _instructionService.ClearWorkInstructionsAsync(scheduleResult.Schedule.WorkStationId);
                    await _materialService.ReleaseReservationsAsync(materialResult.Reservations);
                    await _schedulingService.CancelScheduleAsync(scheduleResult.Schedule.Id);
                    return new ProcessOrderResponseDto
                    {
                        Success = false,
                        Message = taskResult.ErrorMessage,
                        OrderId = request.OrderId
                    };
                }

                // Update order status
                order.UpdateStatus(OrderStatus.InProgress);
                await _orderRepository.UpdateAsync(order);

                // Convert domain objects to DTOs
                return new ProcessOrderResponseDto
                {
                    Success = true,
                    Message = "Order processed successfully",
                    OrderId = request.OrderId,
                    Schedule = MapToScheduleDto(scheduleResult.Schedule),
                    Reservations = materialResult.Reservations.Select(MapToReservationDto).ToList(),
                    Instructions = instructionResult.Instructions.Select(MapToInstructionDto).ToList(),
                    Tasks = taskResult.Tasks.Select(MapToTaskDto).ToList()
                };
            }
            catch (Exception ex)
            {
                return new ProcessOrderResponseDto
                {
                    Success = false,
                    Message = "Internal processing error",
                    OrderId = request.OrderId
                };
            }
        }
        public async Task<List<ProcessOrderResponseDto>> ProcessMultipleOrdersAsync(List<string> orderIds)
        {
            var results = new List<ProcessOrderResponseDto>();

            foreach (var orderId in orderIds)
            {
                var order = await _orderRepository.GetByIdAsync(orderId);
                if (order != null)
                {
                    var request = new ProcessOrderRequestDto
                    {
                        OrderId = order.Id,
                        ProductCode = order.ProductCode,
                        TireSize = order.TireSize,
                        TirePattern = order.TirePattern,
                        Quantity = order.Quantity,
                        RequiredDate = order.RequiredDate,
                        Priority = order.Priority,
                        CustomerCode = order.CustomerCode
                    };

                    var result = await ProcessOrderAsync(request);
                    results.Add(result);
                }
            }

            return results;
        }

        public async Task<OrderStatusDto> GetOrderStatusAsync(string orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                throw new ArgumentException($"Order {orderId} not found");
            }

            return new OrderStatusDto
            {
                OrderId = orderId,
                Status = order.Status.ToString(),
                CurrentStep = await GetCurrentProcessingStep(orderId),
                ProgressPercentage = await CalculateProgressPercentage(orderId)
            };
        }

        // Helper mapping methods
        private ScheduleDto MapToScheduleDto(ProductionSchedule schedule)
        {
            return new ScheduleDto
            {
                ScheduleId = schedule.Id,
                OrderId = schedule.OrderId,
                LineId = schedule.LineId,
                WorkStationId = schedule.WorkStationId,
                ScheduledStartTime = schedule.ScheduledStartTime,
                ScheduledEndTime = schedule.ScheduledEndTime,
                Status = schedule.Status.ToString()
            };
        }

        private MaterialReservationDto MapToReservationDto(MaterialReservation reservation)
        {
            return new MaterialReservationDto
            {
                Id = reservation.Id,
                OrderId = reservation.OrderId,
                MaterialId = reservation.MaterialId,
                MaterialName = reservation.MaterialName,
                ReservedQuantity = reservation.ReservedQuantity,
                Unit = reservation.Unit,
                Status = reservation.Status.ToString()
            };
        }

        private WorkInstructionDto MapToInstructionDto(WorkInstruction instruction)
        {
            return new WorkInstructionDto
            {
                InstructionId = instruction.Id,
                ProcessStep = instruction.ProcessStep,
                Instructions = instruction.Instructions,
                Parameters = instruction.Parameters.Select(p => new ProcessParameterDto
                {
                    ParameterName = p.Name,
                    ParameterValue = p.Value,
                    Unit = p.Unit
                }).ToList()
            };
        }

        private TaskDto MapToTaskDto(ShopfloorTask task)
        {
            return new TaskDto
            {
                TaskId = task.Id,
                OrderId = task.OrderId,
                WorkStationId = task.WorkStationId,
                OperatorId = task.OperatorId,
                TaskType = task.TaskType.ToString(),
                Instructions = task.Instructions,
                Status = task.Status.ToString(),
                CreatedDateTime = task.CreatedDateTime
            };
        }

        private async Task<string> GetCurrentProcessingStep(string orderId) => "Scheduling"; // Placeholder
        private async Task<double> CalculateProgressPercentage(string orderId) => 0.0; // Placeholder


        public interface ISchedulingApplicationService
        {
            Task<ScheduleResponseDto> ScheduleOrderAsync(ScheduleRequestDto request);
            Task<List<MachineDto>> GetAvailableBanburyMachinesAsync();
        }

        public interface IMaterialApplicationService
        {
            Task<MaterialReservationResponseDto> ReserveMaterialsAsync(MaterialReservationRequestDto request);
            Task<MaterialAvailabilityDto> GetMaterialAvailabilityAsync(string materialId);
        }

        public interface ITaskApplicationService
        {
            Task<TaskDispatchResponseDto> DispatchTasksAsync(TaskDispatchRequestDto request);
            Task<List<TaskDto>> GetOperatorTasksAsync(string operatorId);
        }
    }

}

