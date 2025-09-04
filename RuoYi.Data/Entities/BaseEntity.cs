using SqlSugar;

namespace RuoYi.Data.Entities
{
    public class BaseEntity
    {
    }

    public class CreateUserBaseEntity : BaseEntity
    {
        [SugarColumn(ColumnName = "create_by", ColumnDescription = "创建人")]
        public string? CreateBy { get; set; }

        [SugarColumn(ColumnName = "create_time", ColumnDescription = "创建时间")]
        public DateTime? CreateTime { get; set; }
    }

    public class UserBaseEntity : BaseEntity
    {
        [SugarColumn(ColumnName = "create_by", ColumnDescription = "创建人")]
        public string? CreateBy { get; set; }

        [SugarColumn(ColumnName = "create_time", ColumnDescription = "创建时间")]
        public DateTime? CreateTime { get; set; }

        [SugarColumn(ColumnName = "update_by", ColumnDescription = "更新人")]
        public string? UpdateBy { get; set; }

        [SugarColumn(ColumnName = "update_time", ColumnDescription = "更新时间")]
        public DateTime? UpdateTime { get; set; }
    }
}