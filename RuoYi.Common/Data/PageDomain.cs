namespace RuoYi.Common.Data;
public class PageDomain
{
    public int PageNum { get; set; }
    public int PageSize { get; set; }
    public string? OrderByColumn { get; set; }
    public string IsAsc { get; set; } = "asc";
    public string OrderBy { get; set; }
    public string PropertyName { get; set; }
}