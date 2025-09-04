using Microsoft.AspNetCore.Mvc;

namespace RuoYi.Data.Dtos
{
    public class BaseDto
    {
        public string? CreateBy { get; set; }
        public DateTime? CreateTime { get; set; }
        public string? UpdateBy { get; set; }
        public DateTime? UpdateTime { get; set; }
        public string? Remark { get; set; }

        [FromQuery(Name = "")]
        public QueryParam Params { get; set; } = new QueryParam();
    }

    public class QueryParam
    {
        [FromQuery(Name = "params[beginTime]")]
        public DateTime? BeginTime { get; set; }

        [FromQuery(Name = "params[endTime]")]
        public DateTime? EndTime { get; set; }
        public string? DataScopeSql { get; set; }

        [FromQuery(Name = "params[queryType]")]
        public string? QueryType { get; set; }
    }
}