using SqlSugar;
using System;
using System.Collections.Generic;
using RuoYi.Data.Entities;

namespace ZM.Device.Entities
{
    [SugarTable("device_maint", "保养记录表")]
    public class DeviceMaint : UserBaseEntity
    {
        [SugarColumn(ColumnName = "id", ColumnDescription = "自增主键", IsPrimaryKey = true)]
        public long Id { get; set; }

        [SugarColumn(ColumnName = "content", ColumnDescription = "保养内容")]
        public string? Content { get; set; }

        [SugarColumn(ColumnName = "remark", ColumnDescription = "备注")]
        public string? Remark { get; set; }

        [SugarColumn(ColumnName = "image_url", ColumnDescription = "图片")]
        public string? ImageUrl { get; set; }
    }
}