namespace RuoYi.Data.Models
{
    public class Cpu
    {
        public int CpuNum { get; set; }
        public double Total { get; set; }
        public string? Sys { get; set; }
        public string? Used { get; set; }
        public double Wait { get; set; }
        public double Free { get; set; }
    }
}