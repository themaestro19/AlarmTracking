using AlarmTracking.Application.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.DataAccess.ExternalServices
{
    public interface IShopfloorCommunicationService
    {
        Task SendTaskToHMI(string workStationId, ShopfloorTask task);
        Task NotifyOperator(string operatorId, string message);
        Task UpdateTaskStatus(string taskId, Application.Enums.TaskStatus status);
    }
}
