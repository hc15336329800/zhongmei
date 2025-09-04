using RuoYi.Data.Dtos;
using SqlSugar;

namespace ZM.Device.Dtos
{
    public class DeviceInspectionTaskDto : BaseDto
    {
        public long RelatedTaskId { get; set; }
        public long Id { get; set; }
        public string TaskName { get; set; }
        public long? TicketId { get; set; }
        public long? DeviceId { get; set; }
        public string? DeviceName { get; set; }
        public string? Leader { get; set; }
        public string? Executor { get; set; }
        public string? TaskType { get; set; }

        [SugarColumn(ColumnName = "task_status")]
        public string? TaskStatus { get; set; }
        public int? CheckInDeviation { get; set; }
        public DateTime? PlanStartTime { get; set; }
        public DateTime? PlanEndTime { get; set; }
        public DateTime? ActualEndTime { get; set; }
        public string? DeviceIds { get; set; }
        public string? ImageUrl { get; set; }
        public long DevId { get; set; }
        public String? DevName { get; set; }
        public string? Remark { get; set; }

        [SugarColumn(IsIgnore = true)]
        public string LeaderId { get; set; }

        [SugarColumn(IsIgnore = true)]
        public string ExecutorId { get; set; }
    }
}