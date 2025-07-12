using AlarmTracking.Application.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmTracking.DataAccess.ExternalServices
{
    public class ERPIntegrationService : IERPIntegrationService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ERPIntegrationService> _logger;
        private readonly string _erpBaseUrl;

        public ERPIntegrationService(HttpClient httpClient, ILogger<ERPIntegrationService> logger, IConfiguration config)
        {
            _httpClient = httpClient;
            _logger = logger;
            _erpBaseUrl = config.GetValue<string>("ERP:BaseUrl");
        }

        public async Task<ERPOrder> GetOrderFromERPAsync(string orderId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_erpBaseUrl}/api/orders/{orderId}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return System.Text.Json.JsonSerializer.Deserialize<ERPOrder>(json);
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting order {orderId} from ERP");
                throw;
            }
        }

        public async Task UpdateOrderStatusInERPAsync(string orderId, string status)
        {
            try
            {
                var payload = new { OrderId = orderId, Status = status };
                var json = System.Text.Json.JsonSerializer.Serialize(payload);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"{_erpBaseUrl}/api/orders/{orderId}/status", content);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating order status in ERP for order {orderId}");
                throw;
            }
        }

        public async Task<List<ERPOrder>> GetPendingOrdersFromERPAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_erpBaseUrl}/api/orders/pending");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return System.Text.Json.JsonSerializer.Deserialize<List<ERPOrder>>(json);
                }
                return new List<ERPOrder>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pending orders from ERP");
                throw;
            }
        }
    }
}
