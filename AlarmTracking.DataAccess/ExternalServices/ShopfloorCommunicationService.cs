using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.DataAccess.ExternalServices
{
    public class ShopfloorCommunicationService : IShopfloorCommunicationService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ShopfloorCommunicationService> _logger;
        private readonly string _hmiBaseUrl;

        public ShopfloorCommunicationService(HttpClient httpClient, ILogger<ShopfloorCommunicationService> logger, IConfiguration config)
        {
            _httpClient = httpClient;
            _logger = logger;
            _hmiBaseUrl = config.GetValue<string>("HMI:BaseUrl");
        }

        public async Task SendTaskToHMI(string workStationId, Application.Entities.ShopfloorTask task)
        {
            try
            {
                var payload = new
                {
                    TaskId = task.Id,
                    WorkStationId = workStationId,
                    Instructions = task.Instructions,
                    TaskType = task.TaskType.ToString(),
                    OperatorId = task.OperatorId
                };

                var json = System.Text.Json.JsonSerializer.Serialize(payload);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_hmiBaseUrl}/api/workstations/{workStationId}/tasks", content);
                response.EnsureSuccessStatusCode();

                _logger.LogInformation($"Task {task.Id} sent to HMI for workstation {workStationId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending task {task.Id} to HMI");
                throw;
            }
        }

        public async Task NotifyOperator(string operatorId, string message)
        {
            try
            {
                var payload = new { OperatorId = operatorId, Message = message, Timestamp = DateTime.UtcNow };
                var json = System.Text.Json.JsonSerializer.Serialize(payload);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_hmiBaseUrl}/api/operators/{operatorId}/notifications", content);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error notifying operator {operatorId}");
                throw;
            }
        }

        public async Task UpdateTaskStatus(string taskId, Application.Enums.TaskStatus status)
        {
            try
            {
                var payload = new { TaskId = taskId, Status = status.ToString(), UpdatedAt = DateTime.UtcNow };
                var json = System.Text.Json.JsonSerializer.Serialize(payload);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"{_hmiBaseUrl}/api/tasks/{taskId}/status", content);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating task status for task {taskId}");
                throw;
            }
        }
    }
}
