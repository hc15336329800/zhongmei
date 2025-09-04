using System.Collections.Generic;
using RuoYi.Data.Dtos;

namespace ZM.Device.Dtos
{
    public class DeviceInspectionRecordDto : BaseDto
    {
        public long Id { get; set; }
        public long TaskId { get; set; }
        public string TaskName { get; set; }
        public string? Leader { get; set; }
        public string? Executor { get; set; }
        public string? TaskType { get; set; }
        public string? TaskStatus { get; set; }
        public int? CheckInDeviation { get; set; }
        public DateTime? TaskStartTime { get; set; }
        public DateTime? TaskEndTime { get; set; }
        public DateTime? PlanEndTime { get; set; }
        public int? DeviceCount { get; set; }
        public int? DefectTotal { get; set; }
        public int? DefectProcessed { get; set; }
        public string? InspectedDeviceIds { get; set; }
        public string? FinishedDeviceIds { get; set; }
    }
}