using SqlSugar;

namespace RuoYi.Data.Entities
{
    public class TreeEntity : UserBaseEntity
    {
        [SugarColumn(ColumnName = "parent_name", ColumnDescription = "父菜单名称")]
        private string ParentName { get; set; }

        [SugarColumn(ColumnName = "parent_id", ColumnDescription = "父菜单ID")]
        private long ParentId { get; set; }

        [SugarColumn(ColumnName = "order_num", ColumnDescription = "显示顺序")]
        private int? OrderNum { get; set; }

        [SugarColumn(ColumnName = "ancestors", ColumnDescription = "祖级列表")]
        private string Ancestors { get; set; }
    }
}