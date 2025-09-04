using SqlSugar;
using System;
using System.Collections.Generic;
using RuoYi.Data.Entities;

namespace ZM.Device.Entities
{
    [SugarTable("device_inspection_record", "缺陷/维修/抢修记录表（统一结构）")]
    public class DeviceInspectionRecord : UserBaseEntity
    {
        [SugarColumn(ColumnName = "id", ColumnDescription = "主键", IsPrimaryKey = true)]
        public long Id { get; set; }

        [SugarColumn(ColumnName = "task_id", ColumnDescription = "任务ID，关联 device_inspection_task(id)")]
        public long TaskId { get; set; }

        [SugarColumn(ColumnName = "task_name", ColumnDescription = "任务名称")]
        public string TaskName { get; set; }

        [SugarColumn(ColumnName = "leader", ColumnDescription = "负责人")]
        public string? Leader { get; set; }

        [SugarColumn(ColumnName = "executor", ColumnDescription = "执行人")]
        public string? Executor { get; set; }

        [SugarColumn(ColumnName = "task_type", ColumnDescription = "任务类型（巡检、维修、抢修）")]
        public string? TaskType { get; set; }

        [SugarColumn(ColumnName = "task_status", ColumnDescription = "任务状态（待办、在办、办毕）")]
        public string? TaskStatus { get; set; }

        [SugarColumn(ColumnName = "check_in_deviation", ColumnDescription = "签到偏差距离（单位m）")]
        public int? CheckInDeviation { get; set; }

        [SugarColumn(ColumnName = "task_start_time", ColumnDescription = "任务开始时间")]
        public DateTime? TaskStartTime { get; set; }

        [SugarColumn(ColumnName = "task_end_time", ColumnDescription = "任务结束时间")]
        public DateTime? TaskEndTime { get; set; }

        [SugarColumn(ColumnName = "plan_end_time", ColumnDescription = "计划完成时间")]
        public DateTime? PlanEndTime { get; set; }

        [SugarColumn(ColumnName = "device_count", ColumnDescription = "巡检设备数量")]
        public int? DeviceCount { get; set; }

        [SugarColumn(ColumnName = "defect_total", ColumnDescription = "缺陷总个数")]
        public int? DefectTotal { get; set; }

        [SugarColumn(ColumnName = "defect_processed", ColumnDescription = "缺陷已处理数量")]
        public int? DefectProcessed { get; set; }

        [SugarColumn(ColumnName = "inspected_device_ids", ColumnDescription = "巡检设备ID列表（JSON存储）")]
        public string? InspectedDeviceIds { get; set; }

        [SugarColumn(ColumnName = "finished_device_ids", ColumnDescription = "已巡检设备ID列表（JSON存储）")]
        public string? FinishedDeviceIds { get; set; }
    }
}