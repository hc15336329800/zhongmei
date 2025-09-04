using SqlSugar;

namespace RuoYi.Data.Entities
{
    [SugarTable("sys_notice", "通知公告表")]
    public class SysNotice : UserBaseEntity
    {
        [SugarColumn(ColumnName = "notice_id", ColumnDescription = "公告ID", IsPrimaryKey = true, IsIdentity = true)]
        public int NoticeId { get; set; }

        [SugarColumn(ColumnName = "notice_title", ColumnDescription = "公告标题")]
        public string NoticeTitle { get; set; }

        [SugarColumn(ColumnName = "notice_type", ColumnDescription = "公告类型（1通知 2公告）")]
        public string NoticeType { get; set; }

        [SugarColumn(ColumnName = "notice_content", ColumnDescription = "公告内容")]
        public string? NoticeContent { get; set; }

        [SugarColumn(ColumnName = "status", ColumnDescription = "公告状态（0正常 1关闭）")]
        public string? Status { get; set; }
    }
}