using SqlSugar;

namespace RuoYi.Data.Entities
{
    [SugarTable("sys_oper_log", "操作日志记录")]
    public class SysOperLog : BaseEntity
    {
        [SugarColumn(ColumnName = "oper_id", ColumnDescription = "日志主键", IsPrimaryKey = true, IsIdentity = true)]
        public long OperId { get; set; }

        [SugarColumn(ColumnName = "title", ColumnDescription = "模块标题")]
        public string? Title { get; set; }

        [SugarColumn(ColumnName = "business_type", ColumnDescription = "业务类型（0其它 1新增 2修改 3删除）")]
        public int? BusinessType { get; set; }

        [SugarColumn(ColumnName = "method", ColumnDescription = "方法名称")]
        public string? Method { get; set; }

        [SugarColumn(ColumnName = "request_method", ColumnDescription = "请求方式")]
        public string? RequestMethod { get; set; }

        [SugarColumn(ColumnName = "operator_type", ColumnDescription = "操作类别（0其它 1后台用户 2手机端用户）")]
        public int? OperatorType { get; set; }

        [SugarColumn(ColumnName = "oper_name", ColumnDescription = "操作人员")]
        public string? OperName { get; set; }

        [SugarColumn(ColumnName = "dept_name", ColumnDescription = "部门名称")]
        public string? DeptName { get; set; }

        [SugarColumn(ColumnName = "oper_url", ColumnDescription = "请求URL")]
        public string? OperUrl { get; set; }

        [SugarColumn(ColumnName = "oper_ip", ColumnDescription = "主机地址")]
        public string? OperIp { get; set; }

        [SugarColumn(ColumnName = "oper_location", ColumnDescription = "操作地点")]
        public string? OperLocation { get; set; }

        [SugarColumn(ColumnName = "oper_param", ColumnDescription = "请求参数")]
        public string? OperParam { get; set; }

        [SugarColumn(ColumnName = "json_result", ColumnDescription = "返回参数")]
        public string? JsonResult { get; set; }

        [SugarColumn(ColumnName = "status", ColumnDescription = "操作状态（0正常 1异常）")]
        public int? Status { get; set; }

        [SugarColumn(ColumnName = "error_msg", ColumnDescription = "错误消息")]
        public string? ErrorMsg { get; set; }

        [SugarColumn(ColumnName = "oper_time", ColumnDescription = "操作时间")]
        public DateTime? OperTime { get; set; }

        [SugarColumn(ColumnName = "cost_time", ColumnDescription = "消耗时间")]
        public long? CostTime { get; set; }
    }
}