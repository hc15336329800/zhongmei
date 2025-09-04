using SqlSugar;

namespace RuoYi.Data.Entities
{
    [SugarTable("gen_table", "代码生成业务表")]
    public class GenTable : UserBaseEntity
    {
        [SugarColumn(ColumnName = "table_id", IsPrimaryKey = true, IsIdentity = true)]
        public long TableId { get; set; }

        [SugarColumn(ColumnName = "table_name", ColumnDescription = "表名称")]
        public string? TableName { get; set; }

        [SugarColumn(ColumnName = "table_comment", ColumnDescription = "表描述")]
        public string? TableComment { get; set; }

        [SugarColumn(ColumnName = "sub_table_name", ColumnDescription = "关联父表的表名")]
        public string? SubTableName { get; set; }

        [SugarColumn(ColumnName = "sub_table_fk_name", ColumnDescription = "本表关联父表的外键名")]
        public string? SubTableFkName { get; set; }

        [SugarColumn(ColumnName = "class_name", ColumnDescription = "实体类名称(首字母大写)")]
        public string? ClassName { get; set; }

        [SugarColumn(ColumnName = "tpl_category", ColumnDescription = "使用的模板")]
        public string? TplCategory { get; set; }

        [SugarColumn(ColumnName = "package_name", ColumnDescription = "生成包路径")]
        public string? PackageName { get; set; }

        [SugarColumn(ColumnName = "module_name", ColumnDescription = "生成模块名")]
        public string? ModuleName { get; set; }

        [SugarColumn(ColumnName = "business_name", ColumnDescription = "生成业务名")]
        public string? BusinessName { get; set; }

        [SugarColumn(ColumnName = "function_name", ColumnDescription = "生成功能名")]
        public string? FunctionName { get; set; }

        [SugarColumn(ColumnName = "function_author", ColumnDescription = "生成作者")]
        public string? FunctionAuthor { get; set; }

        [SugarColumn(ColumnName = "gen_type", ColumnDescription = "生成代码方式")]
        public string? GenType { get; set; }

        [SugarColumn(ColumnName = "gen_path", ColumnDescription = "生成路径")]
        public string? GenPath { get; set; }

        [SugarColumn(ColumnName = "options", ColumnDescription = "其它生成选项")]
        public string? Options { get; set; }

        [SugarColumn(ColumnName = "remark", ColumnDescription = "备注")]
        public string? Remark { get; set; }

        [Navigate(NavigateType.OneToMany, nameof(GenTableColumn.TableId))]
        public List<GenTableColumn>? Columns { get; set; }

#region 非DB字段
        [SugarColumn(IsIgnore = true)]
        public GenTableColumn? PkColumn { get; set; }

        [SugarColumn(IsIgnore = true)]
        public GenTable? SubTable { get; set; }

#endregion
#region methods
        public static string[] BASE_ENTITY =
        {
            "CreateBy",
            "CreateTime",
            "UpdateBy",
            "UpdateTime",
            "Remark"
        };
        public static string[] TREE_ENTITY =
        {
            "ParentName",
            "ParentId",
            "OrderNum",
            "Ancestors",
            "Children"
        };
        public bool IsCrud()
        {
            return !string.IsNullOrEmpty(TplCategory) && "crud".Equals(TplCategory);
        }

        public bool IsSub()
        {
            return !string.IsNullOrEmpty(TplCategory) && "sub".Equals(TplCategory);
        }

        public bool IsTree()
        {
            return IsTree(TplCategory);
        }

        public bool IsTree(string tplCategory)
        {
            return !string.IsNullOrEmpty(tplCategory) && "tree".Equals(TplCategory);
        }

        public bool IsSuperColumn(string netField)
        {
            return IsSuperColumn(TplCategory, netField);
        }

        public bool IsSuperColumn(string tplCategory, string netField)
        {
            if (IsTree(tplCategory))
            {
                return TREE_ENTITY.Contains(netField) || BASE_ENTITY.Contains(netField);
            }

            return BASE_ENTITY.Contains(netField);
        }
#endregion
    }
}