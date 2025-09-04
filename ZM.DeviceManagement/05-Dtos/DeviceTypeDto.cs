using RuoYi.Data.Dtos;
using SqlSugar;

namespace RuoYi.Device.Entities
{
    public class DeviceTypeDto : BaseDto
    {
        public long Id { get; set; }
        public long ParentId { get; set; }
        public string? Ancestors { get; set; }
        public string? DeptName { get; set; }
        public int? OrderNum { get; set; }
        public string? Leader { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Status { get; set; }
        public string? DelFlag { get; set; }
        public long? TenantId { get; set; }
        public List<DeviceTypeDto> Children { get; set; }
    }
}