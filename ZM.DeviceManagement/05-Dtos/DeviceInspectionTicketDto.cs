using System.Collections.Generic;
using RuoYi.Data.Dtos;

namespace ZM.Device.Dtos
{
    public class DeviceInspectionTicketDto : BaseDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string? DeviceIds { get; set; }
        public string? Number { get; set; }
    }
}