namespace RuoYi.Data.Models;
public class SysUserOnline
{
    public string? TokenId { get; set; }
    public string? DeptName { get; set; }
    public string? UserName { get; set; }
    public string? Ipaddr { get; set; }
    public string? LoginLocation { get; set; }
    public string? Browser { get; set; }
    public string? Os { get; set; }
    public long LoginTime { get; set; }
}