using RuoYi.Data.Entities;
using SqlSugar;

namespace RuoYi.Device.Entities
{
    [SugarTable("device_type", "设备分类表")]
    public class DeviceType : UserBaseEntity
    {
        [SugarColumn(ColumnName = "id", ColumnDescription = "ID", IsPrimaryKey = true, IsIdentity = true)]
        public long Id { get; set; }

        [SugarColumn(ColumnName = "parent_id", ColumnDescription = "父ID")]
        public long ParentId { get; set; }

        [SugarColumn(ColumnName = "ancestors", ColumnDescription = "祖级列表")]
        public string? Ancestors { get; set; }

        [SugarColumn(ColumnName = "dept_name", ColumnDescription = "名称")]
        public string? DeptName { get; set; }

        [SugarColumn(ColumnName = "order_num", ColumnDescription = "显示顺序")]
        public int? OrderNum { get; set; }

        [SugarColumn(ColumnName = "leader", ColumnDescription = "负责人")]
        public string? Leader { get; set; }

        [SugarColumn(ColumnName = "phone", ColumnDescription = "联系电话")]
        public string? Phone { get; set; }

        [SugarColumn(ColumnName = "email", ColumnDescription = "邮箱")]
        public string? Email { get; set; }

        [SugarColumn(ColumnName = "status", ColumnDescription = "部门状态:0正常,1停用")]
        public string? Status { get; set; }

        [SugarColumn(ColumnName = "del_flag", ColumnDescription = "删除标志（0代表存在 2代表删除）")]
        public string? DelFlag { get; set; }

        [SugarColumn(ColumnName = "tenant_id", ColumnDescription = "租户ID（集团ID）")]
        public long? TenantId { get; set; }

        [SugarColumn(IsIgnore = true)]
        public List<DeviceType> Children { get; set; } = new List<DeviceType>();
    }
}