using SqlSugar;

namespace RuoYi.Data.Entities
{
    [SugarTable("gen_table_column", "代码生成业务表字段")]
    public class GenTableColumn : UserBaseEntity
    {
        [SugarColumn(ColumnName = "column_id", IsPrimaryKey = true, IsIdentity = true)]
        public long ColumnId { get; set; }

        [SugarColumn(ColumnName = "table_id")]
        public long? TableId { get; set; }

        [SugarColumn(ColumnName = "column_name")]
        public string ColumnName { get; set; }

        [SugarColumn(ColumnName = "column_comment")]
        public string ColumnComment { get; set; }

        [SugarColumn(ColumnName = "column_type")]
        public string ColumnType { get; set; }

        [SugarColumn(ColumnName = "net_type")]
        public string NetType { get; set; }

        [SugarColumn(ColumnName = "net_field")]
        public string NetField { get; set; }

        [SugarColumn(ColumnName = "is_pk")]
        public string? IsPk { get; set; }

        [SugarColumn(ColumnName = "is_increment")]
        public string? IsIncrement { get; set; }

        [SugarColumn(ColumnName = "is_required")]
        public string? IsRequired { get; set; }

        [SugarColumn(ColumnName = "is_insert")]
        public string? IsInsert { get; set; }

        [SugarColumn(ColumnName = "is_edit")]
        public string? IsEdit { get; set; }

        [SugarColumn(ColumnName = "is_list")]
        public string? IsList { get; set; }

        [SugarColumn(ColumnName = "is_query")]
        public string? IsQuery { get; set; }

        [SugarColumn(ColumnName = "query_type")]
        public string QueryType { get; set; }

        [SugarColumn(ColumnName = "html_type")]
        public string HtmlType { get; set; }

        [SugarColumn(ColumnName = "dict_type")]
        public string DictType { get; set; }

        [SugarColumn(ColumnName = "sort")]
        public int? Sort { get; set; }

#region methods
        public bool Is_List()
        {
            return IsYes(IsList);
        }

        public bool Is_Pk()
        {
            return IsYes(IsPk);
        }

        public bool Is_Increment()
        {
            return IsYes(IsIncrement);
        }

        public bool Is_Required()
        {
            return IsYes(IsRequired);
        }

        public bool Is_Query()
        {
            return IsYes(IsQuery);
        }

        public bool Is_Insert()
        {
            return IsYes(IsInsert);
        }

        public bool Is_Edit()
        {
            return IsYes(IsEdit);
        }

        public static bool IsYes(string? yesNo)
        {
            return "1".Equals(yesNo);
        }

        private static List<string> _UsableColumns = new List<string>
        {
            "ParentId",
            "OrderNum",
            "Remark"
        };
        public bool IsUsableColumn(string netField)
        {
            return !string.IsNullOrEmpty(netField) && _UsableColumns.Contains(netField);
        }

        public bool IsUsableColumn()
        {
            return IsUsableColumn(NetField ?? "");
        }

        private static List<string> _SuperColumns = new List<string>
        {
            "CreateBy",
            "CreateTime",
            "UpdateBy",
            "UpdateTime",
            "Remark",
            "ParentName",
            "ParentId",
            "OrderNum",
            "Ancestors"
        };
        public bool IsSuperColumn(string netField)
        {
            return !string.IsNullOrEmpty(netField) && _SuperColumns.Contains(netField);
        }

        public bool IsSuperColumn()
        {
            return IsSuperColumn(NetField ?? "");
        }

        public string NetFieldLower()
        {
            if (!string.IsNullOrEmpty(NetField))
            {
                return string.Concat(NetField.First().ToString().ToLower(), NetField.AsSpan(1));
            }

            return NetField;
        }
#endregion
    }
}