using SqlSugar;
using System;
using System.Collections.Generic;
using RuoYi.Data.Entities;

namespace ZM.Device.Entities
{
    [SugarTable("device_management_maint", "设备信息与设备保养中间表")]
    public class DeviceManagementMaint : BaseEntity
    {
        [SugarColumn(ColumnName = "device_id", ColumnDescription = "设备CodeID", IsPrimaryKey = true)]
        public long DeviceId { get; set; }

        [SugarColumn(ColumnName = "maint_id", ColumnDescription = "保养CodeID", IsPrimaryKey = true)]
        public long MaintId { get; set; }

        [SugarColumn(ColumnName = "tenant_id", ColumnDescription = "所属集团ID")]
        public long? TenantId { get; set; }
    }
}