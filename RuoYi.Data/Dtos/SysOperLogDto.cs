using RuoYi.Data.Attributes;

namespace RuoYi.Data.Dtos
{
    public class SysOperLogDto : BaseDto
    {
        [Excel(Name = "操作序号")]
        public long OperId { get; set; }

        [Excel(Name = "操作模块")]
        public string? Title { get; set; }
        public int? BusinessType { get; set; }

        [Excel(Name = "业务类型")]
        public string? BusinessTypeDesc { get; set; }
        public string? Method { get; set; }
        public string? RequestMethod { get; set; }
        public int? OperatorType { get; set; }
        public string? OperName { get; set; }
        public string? DeptName { get; set; }
        public string? OperUrl { get; set; }
        public string? OperIp { get; set; }
        public string? OperLocation { get; set; }
        public string? OperParam { get; set; }
        public string? JsonResult { get; set; }
        public int? Status { get; set; }
        public string? ErrorMsg { get; set; }
        public DateTime? OperTime { get; set; }
        public long? CostTime { get; set; }
    }
}