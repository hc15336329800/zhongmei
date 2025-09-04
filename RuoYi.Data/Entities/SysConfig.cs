using SqlSugar;

namespace RuoYi.Data.Entities
{
    [SugarTable("sys_config", "参数配置表")]
    public class SysConfig : UserBaseEntity
    {
        [SugarColumn(ColumnName = "config_id", ColumnDescription = "参数主键", IsPrimaryKey = true, IsIdentity = true)]
        public int ConfigId { get; set; }

        [SugarColumn(ColumnName = "config_name", ColumnDescription = "参数名称")]
        public string? ConfigName { get; set; }

        [SugarColumn(ColumnName = "config_key", ColumnDescription = "参数键名")]
        public string? ConfigKey { get; set; }

        [SugarColumn(ColumnName = "config_value", ColumnDescription = "参数键值")]
        public string? ConfigValue { get; set; }

        [SugarColumn(ColumnName = "config_type", ColumnDescription = "系统内置（Y是 N否）")]
        public string? ConfigType { get; set; }

        [SugarColumn(ColumnName = "remark", ColumnDescription = "备注")]
        public string? Remark { get; set; }
    }
}