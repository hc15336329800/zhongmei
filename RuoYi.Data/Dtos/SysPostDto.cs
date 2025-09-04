using RuoYi.Data.Attributes;
using System.ComponentModel.DataAnnotations;

namespace RuoYi.Data.Dtos
{
    public class SysPostDto : BaseDto
    {
        [Excel(Name = "岗位序号")]
        public long? PostId { get; set; }

        [Excel(Name = "岗位编码")]
        [Required(ErrorMessage = "岗位编码不能为空"), MaxLength(64, ErrorMessage = "岗位编码长度不能超过64个字符")]
        public string? PostCode { get; set; }
        public string? PostCodeLike { get; set; }

        [Excel(Name = "岗位名称")]
        [Required(ErrorMessage = "岗位名称不能为空"), MaxLength(50, ErrorMessage = "岗位名称长度不能超过50个字符")]
        public string? PostName { get; set; }
        public string? PostNameLike { get; set; }

        [Excel(Name = "岗位排序")]
        [Required(ErrorMessage = "显示顺序不能为空")]
        public int? PostSort { get; set; }
        public string? Status { get; set; }

        [Excel(Name = "状态")]
        public string? StatusDesc { get; set; }
        public string? UserName { get; set; }
        public long? UserId { get; set; }
    }
}