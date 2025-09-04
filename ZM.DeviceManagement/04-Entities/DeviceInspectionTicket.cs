using SqlSugar;
using System;
using System.Collections.Generic;
using RuoYi.Data.Entities;

namespace ZM.Device.Entities
{
    [SugarTable("device_inspection_ticket", "设备巡检操作票表")]
    public class DeviceInspectionTicket : BaseEntity
    {
        [SugarColumn(ColumnName = "id", ColumnDescription = "主键ID（不自增）", IsPrimaryKey = true, IsIdentity = false)]
        public long Id { get; set; }

        [SugarColumn(ColumnName = "name", ColumnDescription = "操作票名称")]
        public string Name { get; set; }

        [SugarColumn(ColumnName = "device_ids", ColumnDescription = "设备ID列表（JSON格式）")]
        public string? DeviceIds { get; set; }

        [SugarColumn(ColumnName = "number", ColumnDescription = "设备数量")]
        public string? Number { get; set; }
    }
}