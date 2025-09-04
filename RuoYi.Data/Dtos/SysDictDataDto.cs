using RuoYi.Data.Attributes;
using System.ComponentModel.DataAnnotations;

namespace RuoYi.Data.Dtos
{
    public class SysDictDataDto : BaseDto
    {
        [Excel(Name = "字典编码")]
        public long DictCode { get; set; }

        [Excel(Name = "字典排序")]
        public int? DictSort { get; set; }

        [Excel(Name = "字典标签")]
        [Required(ErrorMessage = "字典标签不能为空"), StringLength(100, ErrorMessage = "字典标签长度不能超过100个字符")]
        public string? DictLabel { get; set; }

        [Excel(Name = "字典键值")]
        [Required(ErrorMessage = "字典键值不能为空"), StringLength(100, ErrorMessage = "字典键值长度不能超过100个字符")]
        public string? DictValue { get; set; }

        [Excel(Name = "字典类型")]
        [Required(ErrorMessage = "字典类型不能为空"), StringLength(100, ErrorMessage = "字典类型长度不能超过100个字符")]
        public string? DictType { get; set; }

        [StringLength(100, ErrorMessage = "样式属性长度不能超过100个字符")]
        public string? CssClass { get; set; }
        public string? ListClass { get; set; }
        public string? IsDefault { get; set; }

        [Excel(Name = "是否默认")]
        public string? IsDefaultDesc { get; set; }
        public string? Status { get; set; }

        [Excel(Name = "状态")]
        public string? StatusDesc { get; set; }
    }
}