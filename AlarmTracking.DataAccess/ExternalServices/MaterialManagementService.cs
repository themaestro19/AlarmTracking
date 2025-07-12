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
    public class MaterialManagementService : IMaterialManagementService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<MaterialManagementService> _logger;
        private readonly string _warehouseBaseUrl;

        public MaterialManagementService(HttpClient httpClient, ILogger<MaterialManagementService> logger, IConfiguration config)
        {
            _httpClient = httpClient;
            _logger = logger;
            _warehouseBaseUrl = config.GetValue<string>("Warehouse:BaseUrl");
        }

        public async Task<List<Material>> GetAvailableMaterialsAsync(string materialId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_warehouseBaseUrl}/api/materials/{materialId}/available");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return System.Text.Json.JsonSerializer.Deserialize<List<Material>>(json);
                }
                return new List<Material>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting available materials for {materialId}");
                throw;
            }
        }

        public async Task<BillOfMaterial> GetBillOfMaterialsAsync(string productCode)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_warehouseBaseUrl}/api/bom/{productCode}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return System.Text.Json.JsonSerializer.Deserialize<BillOfMaterial>(json);
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting BOM for product {productCode}");
                throw;
            }
        }

        public async Task ReserveMaterialAsync(string materialId, double quantity, string orderId)
        {
            try
            {
                var payload = new { MaterialId = materialId, Quantity = quantity, OrderId = orderId, ReservedAt = DateTime.UtcNow };
                var json = System.Text.Json.JsonSerializer.Serialize(payload);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_warehouseBaseUrl}/api/materials/reserve", content);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error reserving material {materialId}");
                throw;
            }
        }

        public async Task ReleaseMaterialReservationAsync(string reservationId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_warehouseBaseUrl}/api/reservations/{reservationId}");
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error releasing reservation {reservationId}");
                throw;
            }
        }
    }
}
