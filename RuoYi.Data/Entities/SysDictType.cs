using SqlSugar;

namespace RuoYi.Data.Entities
{
    [SugarTable("sys_dict_type", "字典类型表")]
    public class SysDictType : UserBaseEntity
    {
        [SugarColumn(ColumnName = "dict_id", ColumnDescription = "字典主键", IsPrimaryKey = true, IsIdentity = true)]
        public long DictId { get; set; }

        [SugarColumn(ColumnName = "dict_name", ColumnDescription = "字典名称")]
        public string? DictName { get; set; }

        [SugarColumn(ColumnName = "dict_type", ColumnDescription = "字典类型")]
        public string? DictType { get; set; }

        [SugarColumn(ColumnName = "status", ColumnDescription = "状态（0正常 1停用）")]
        public string? Status { get; set; }

        [SugarColumn(ColumnName = "remark", ColumnDescription = "备注")]
        public string? Remark { get; set; }
    }
}