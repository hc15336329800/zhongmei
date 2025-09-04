using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZM.Device._05_Dtos
{
    public class SubmitDeviceStatusDto
    {
        public long TaskId { get; set; }
        public long Id { get; set; }
        public string DevStatus { get; set; }
        public bool IsCompleted { get; set; }
        public string Remark { get; set; }
        public string? ImageUrl { get; set; }
    }
}