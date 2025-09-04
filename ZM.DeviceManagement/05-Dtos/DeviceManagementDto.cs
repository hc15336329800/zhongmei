using RuoYi.Data.Dtos;
using RuoYi.Data.Entities;
using SqlSugar;

namespace ZM.Device.Entities
{
    public class DeviceManagementDto : BaseDto
    {
        public int? MaintenanceCountdown { get; set; }
        public DateTime? LastMaintenanceTime { get; set; }
        public long Id { get; set; }
        public string Label { get; set; }
        public string? DeviceType { get; set; }
        public string? Model { get; set; }
        public decimal? Capacity { get; set; }
        public int Quantity { get; set; }
        public decimal? Weight { get; set; }
        public string? Manufacturer { get; set; }
        public DateTime? InstallDate { get; set; }
        public decimal? RatedCurrent { get; set; }
        public decimal? RatedVoltage { get; set; }
        public string? Status { get; set; }
        public decimal? TempControl { get; set; }
        public int? MaintenanceCycle { get; set; }
        public DateTime? WarrantyPeriod { get; set; }
        public int ProcessId { get; set; }
    }
}