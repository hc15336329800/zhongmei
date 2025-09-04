using RuoYi.Data.Dtos;
using SqlSugar;

namespace ZM.Device.Dtos
{
    public class DeviceDefectRecordDto : BaseDto
    {
        public long Id { get; set; }
        public long DeviceId { get; set; }
        public long TaskId { get; set; }
        public string? DevicePath { get; set; }
        public string DeviceName { get; set; }
        public string DefectName { get; set; }
        public string? DefectDesc { get; set; }
        public string? DefectStatus { get; set; }
        public string? DefectCategory { get; set; }
        public string? SeverityLevel { get; set; }
        public string? Suggestion { get; set; }
        public DateTime? DiscoveryTime { get; set; }
        public DateTime? FixTime { get; set; }
        public string? FixPerson { get; set; }
        public DateTime? FixDeadline { get; set; }
        public string? ImageUrl { get; set; }
    }
}