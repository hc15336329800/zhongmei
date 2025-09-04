using RuoYi.Data.Entities;
using SqlSugar;

namespace RuoYi.Device.Entities
{
    [SugarTable("device_management_type", TableDescription = "设备管理与设备类型中间表")]
    public class DeviceManagementType : UserBaseEntity
    {
        [SugarColumn(ColumnName = "device_id", IsPrimaryKey = true, ColumnDescription = "设备ID")]
        public long DeviceId { get; set; }

        [SugarColumn(ColumnName = "devicetype_id", IsPrimaryKey = true, ColumnDescription = "设备类型ID")]
        public long DeviceTypeId { get; set; }

        [SugarColumn(ColumnName = "tenant_id", ColumnDescription = "租户ID", IsNullable = true)]
        public long TenantId { get; set; } = 0;
    }
}