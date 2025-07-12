using AlarmTracking.Application.DTOs;
using AlarmTracking.WebService.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.Application.Services
{
    public interface IProductionOrderApplicationService
    {
        Task<ProcessOrderResponseDto> ProcessOrderAsync(ProcessOrderRequestDto request);
        Task<List<ProcessOrderResponseDto>> ProcessMultipleOrdersAsync(List<string> orderIds);
        Task<OrderStatusDto> GetOrderStatusAsync(string orderId);
    }
}
