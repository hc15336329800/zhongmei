using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RuoYi.Data.Dtos;

namespace RuoYi.Device.Entities
{
    public class DeviceManagementTypeDto : BaseDto
    {
        public long DeviceId { get; set; }
        public long DeviceTypeId { get; set; }
        public long TenantId { get; set; }
    }
}