using RuoYi.Data.Entities;
using SqlSugar;

namespace ZM.Device.Entities
{
    [SugarTable("device_management", "设备管理表")]
    public class DeviceManagement : UserBaseEntity
    {
        [SugarColumn(ColumnName = "last_maintenance_time", ColumnDescription = "上次保养时间，可为空")]
        public DateTime? LastMaintenanceTime { get; set; }

        [SugarColumn(ColumnName = "id", ColumnDescription = "自增主键", IsPrimaryKey = true)]
        public long Id { get; set; }

        [SugarColumn(ColumnName = "remark", ColumnDescription = "保养内容")]
        public string? Remark { get; set; }

        [SugarColumn(ColumnName = "label", ColumnDescription = "设备名称，必填")]
        public string Label { get; set; }

        [SugarColumn(ColumnName = "device_type", ColumnDescription = "设备类型，可为空")]
        public string? DeviceType { get; set; }

        [SugarColumn(ColumnName = "model", ColumnDescription = "型号规格，可为空")]
        public string? Model { get; set; }

        [SugarColumn(ColumnName = "capacity", ColumnDescription = "额定容量（单位L），可为空")]
        public decimal? Capacity { get; set; }

        [SugarColumn(ColumnName = "quantity", ColumnDescription = "设备数量，必填")]
        public int Quantity { get; set; }

        [SugarColumn(ColumnName = "weight", ColumnDescription = "设备重量（单位KG），可为空")]
        public decimal? Weight { get; set; }

        [SugarColumn(ColumnName = "manufacturer", ColumnDescription = "生产厂家，可为空")]
        public string? Manufacturer { get; set; }

        [SugarColumn(ColumnName = "install_date", ColumnDescription = "安装时间，可为空")]
        public DateTime? InstallDate { get; set; }

        [SugarColumn(ColumnName = "rated_current", ColumnDescription = "额定电流（单位A），可为空")]
        public decimal? RatedCurrent { get; set; }

        [SugarColumn(ColumnName = "rated_voltage", ColumnDescription = "额定电压（单位V），可为空")]
        public decimal? RatedVoltage { get; set; }

        [SugarColumn(ColumnName = "status", ColumnDescription = "设备当前状态（如运行、待机、故障），可为空")]
        public string? Status { get; set; }

        [SugarColumn(ColumnName = "temp_control", ColumnDescription = "温控要求（单位℃），可为空")]
        public decimal? TempControl { get; set; }

        [SugarColumn(ColumnName = "maintenance_cycle", ColumnDescription = "维护周期（单位天），可为空")]
        public int? MaintenanceCycle { get; set; }

        [SugarColumn(ColumnName = "warranty_period", ColumnDescription = "原保期（保修到期时间），可为空")]
        public DateTime? WarrantyPeriod { get; set; }

        [SugarColumn(ColumnName = "process_id", ColumnDescription = "设备所属工序ID，必填")]
        public int ProcessId { get; set; }
    }
}