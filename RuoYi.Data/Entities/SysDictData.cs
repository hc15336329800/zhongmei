using SqlSugar;

namespace RuoYi.Data.Entities
{
    [SugarTable("sys_dict_data", "字典数据表")]
    public class SysDictData : UserBaseEntity
    {
        [SugarColumn(ColumnName = "dict_code", ColumnDescription = "字典编码", IsPrimaryKey = true, IsIdentity = true)]
        public long DictCode { get; set; }

        [SugarColumn(ColumnName = "dict_sort", ColumnDescription = "字典排序")]
        public int? DictSort { get; set; }

        [SugarColumn(ColumnName = "dict_label", ColumnDescription = "字典标签")]
        public string? DictLabel { get; set; }

        [SugarColumn(ColumnName = "dict_value", ColumnDescription = "字典键值")]
        public string? DictValue { get; set; }

        [SugarColumn(ColumnName = "dict_type", ColumnDescription = "字典类型")]
        public string? DictType { get; set; }

        [SugarColumn(ColumnName = "css_class", ColumnDescription = "样式属性（其他样式扩展）")]
        public string? CssClass { get; set; }

        [SugarColumn(ColumnName = "list_class", ColumnDescription = "表格回显样式")]
        public string? ListClass { get; set; }

        [SugarColumn(ColumnName = "is_default", ColumnDescription = "是否默认（Y是 N否）")]
        public string? IsDefault { get; set; }

        [SugarColumn(ColumnName = "status", ColumnDescription = "状态（0正常 1停用）")]
        public string? Status { get; set; }

        [SugarColumn(ColumnName = "remark", ColumnDescription = "备注")]
        public string? Remark { get; set; }
    }
}