using SqlSugar;
using System;
using System.Collections.Generic;
using RuoYi.Data.Entities;

namespace ZM.Device.Entities
{
    [SugarTable("device_defect_record", "设备缺陷记录表")]
    public class DeviceDefectRecord : UserBaseEntity
    {
        [SugarColumn(ColumnName = "id", ColumnDescription = "主键ID", IsPrimaryKey = true)]
        public long Id { get; set; }

        [SugarColumn(ColumnName = "device_id", ColumnDescription = "设备ID")]
        public long DeviceId { get; set; }

        [SugarColumn(ColumnName = "task_id", ColumnDescription = "关联任务ID")]
        public long TaskId { get; set; }

        [SugarColumn(ColumnName = "device_path", ColumnDescription = "设备路径（厂房/车间/工序）")]
        public string? DevicePath { get; set; }

        [SugarColumn(ColumnName = "device_name", ColumnDescription = "设备名称")]
        public string DeviceName { get; set; }

        [SugarColumn(ColumnName = "defect_name", ColumnDescription = "缺陷名称")]
        public string DefectName { get; set; }

        [SugarColumn(ColumnName = "defect_desc", ColumnDescription = "缺陷描述")]
        public string? DefectDesc { get; set; }

        [SugarColumn(ColumnName = "defect_status", ColumnDescription = "缺陷状态（已处理/未处理）")]
        public string? DefectStatus { get; set; }

        [SugarColumn(ColumnName = "defect_category", ColumnDescription = "缺陷类别")]
        public string? DefectCategory { get; set; }

        [SugarColumn(ColumnName = "severity_level", ColumnDescription = "严重等级")]
        public string? SeverityLevel { get; set; }

        [SugarColumn(ColumnName = "suggestion", ColumnDescription = "处理建议")]
        public string? Suggestion { get; set; }

        [SugarColumn(ColumnName = "discovery_time", ColumnDescription = "发现时间")]
        public DateTime? DiscoveryTime { get; set; }

        [SugarColumn(ColumnName = "fix_time", ColumnDescription = "消缺时间")]
        public DateTime? FixTime { get; set; }

        [SugarColumn(ColumnName = "fix_person", ColumnDescription = "消缺人员")]
        public string? FixPerson { get; set; }

        [SugarColumn(ColumnName = "fix_deadline", ColumnDescription = "消缺期限")]
        public DateTime? FixDeadline { get; set; }

        [SugarColumn(ColumnName = "image_url", ColumnDescription = "缺陷图片路径")]
        public string? ImageUrl { get; set; }
        public string? Remark { get; set; }
    }
}