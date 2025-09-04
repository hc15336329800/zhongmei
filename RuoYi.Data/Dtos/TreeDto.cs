using SqlSugar;

namespace RuoYi.Data.Dtos
{
    public class TreeDto : BaseDto
    {
        private string ParentName { get; set; }
        private long ParentId { get; set; }
        private int? OrderNum { get; set; }
        private string Ancestors { get; set; }
    }
}