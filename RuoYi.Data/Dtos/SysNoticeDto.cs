using System.ComponentModel.DataAnnotations;

namespace RuoYi.Data.Dtos
{
    public class SysNoticeDto : BaseDto
    {
        public int? NoticeId { get; set; }

        [Required(ErrorMessage = "公告标题不能为空"), MaxLength(50, ErrorMessage = "公告标题不能超过50个字符")]
        public string? NoticeTitle { get; set; }
        public string? NoticeType { get; set; }
        public string? NoticeContent { get; set; }
        public string? Status { get; set; }
    }
}