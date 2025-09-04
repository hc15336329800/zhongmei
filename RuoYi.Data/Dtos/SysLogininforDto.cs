namespace RuoYi.Data.Dtos
{
    public class SysLogininforDto : BaseDto
    {
        public long InfoId { get; set; }
        public string? UserName { get; set; }
        public string? Ipaddr { get; set; }
        public string? LoginLocation { get; set; }
        public string? Browser { get; set; }
        public string? Os { get; set; }
        public string? Status { get; set; }
        public string? Msg { get; set; }
        public DateTime? LoginTime { get; set; }
    }
}