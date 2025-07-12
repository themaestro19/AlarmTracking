using AlarmTracking.Application.Contracts.Persistence;
using AlarmTracking.Application.Models;
using AlarmTracking.Application.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlarmTracking.Domain.Repositories;

namespace AlarmTracking.Application.Services
{
    public class MaterialReservationDomainService : IMaterialReservationDomainService
    {
        private readonly IMaterialRepository _materialRepository;
        private readonly IBillOfMaterialRepository _bomRepository;
        private readonly IMaterialReservationRepository _reservationRepository;

        public MaterialReservationDomainService(
            IMaterialRepository materialRepository,
            IBillOfMaterialRepository bomRepository,
            IMaterialReservationRepository reservationRepository)
        {
            _materialRepository = materialRepository;
            _bomRepository = bomRepository;
            _reservationRepository = reservationRepository;
        }

        public async Task<MaterialReservationResult> ReserveMaterialsAsync(ProductionOrder order, ProductionSchedule schedule)
        {
            var bom = await _bomRepository.GetByProductCodeAsync(order.ProductCode);
            if (bom == null)
            {
                return MaterialReservationResult.CreateFailure("Bill of Materials not found");
            }

            var reservations = new List<MaterialReservation>();

            foreach (var bomItem in bom.Items)
            {
                var requiredQuantity = bomItem.Quantity * order.Quantity;
                var availableMaterials = await _materialRepository.GetAvailableMaterialsAsync(bomItem.MaterialId);

                var totalAvailable = availableMaterials.Sum(m => m.AvailableQuantity);
                if (totalAvailable < requiredQuantity)
                {
                    return MaterialReservationResult.CreateFailure($"Insufficient material: {bomItem.MaterialName}");
                }

                // Apply FIFO allocation
                var allocatedMaterials = AllocateMaterialsFIFO(availableMaterials, requiredQuantity);

                foreach (var allocation in allocatedMaterials)
                {
                    var reservation = MaterialReservation.Create(
                        order.Id,
                        allocation.MaterialId,
                        allocation.MaterialName,
                        allocation.Quantity,
                        allocation.Unit);

                    await _reservationRepository.AddAsync(reservation);
                    reservations.Add(reservation);
                }
            }

            return MaterialReservationResult.CreateSuccess(reservations);
        }

        public async Task ReleaseReservationsAsync(List<MaterialReservation> reservations)
        {
            foreach (var reservation in reservations)
            {
                reservation.Release();
                await _reservationRepository.UpdateAsync(reservation);
            }
        }

        private List<MaterialAllocation> AllocateMaterialsFIFO(List<Material> materials, double requiredQuantity)
        {
            var allocations = new List<MaterialAllocation>();
            var remaining = requiredQuantity;

            foreach (var material in materials.OrderBy(m => m.ExpiryDate))
            {
                if (remaining <= 0) break;

                var allocated = Math.Min(material.AvailableQuantity, remaining);
                allocations.Add(new MaterialAllocation
                {
                    MaterialId = material.Id,
                    MaterialName = material.Name,
                    Quantity = allocated,
                    Unit = material.Unit
                });

                remaining -= allocated;
            }

            return allocations;
        }
    }
}
