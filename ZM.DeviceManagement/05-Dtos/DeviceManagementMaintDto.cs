using System.Collections.Generic;
using RuoYi.Data.Dtos;

namespace ZM.Device.Dtos
{
    public class DeviceManagementMaintDto : BaseDto
    {
        public long DeviceId { get; set; }
        public long MaintId { get; set; }
        public long? TenantId { get; set; }
    }
}