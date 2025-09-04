using System.Collections.Generic;
using RuoYi.Data.Dtos;

namespace ZM.Device.Dtos
{
    public class DeviceMaintDto : BaseDto
    {
        public long DeviceId { get; set; }
        public long Id { get; set; }
        public string? Content { get; set; }
        public string? Remark { get; set; }
        public string? ImageUrl { get; set; }
    }
}