using SqlSugar;
using System;
using System.Collections.Generic;
using RuoYi.Data.Entities;

namespace RuoYi.Data.Entities
{
    [SugarTable("sys_post", "岗位信息表")]
    public class SysPost : UserBaseEntity
    {
        [SugarColumn(ColumnName = "post_id", ColumnDescription = "岗位ID", IsPrimaryKey = true, IsIdentity = true)]
        public long PostId { get; set; }

        [SugarColumn(ColumnName = "post_code", ColumnDescription = "岗位编码")]
        public string PostCode { get; set; }

        [SugarColumn(ColumnName = "post_name", ColumnDescription = "岗位名称")]
        public string PostName { get; set; }

        [SugarColumn(ColumnName = "post_sort", ColumnDescription = "显示顺序")]
        public int PostSort { get; set; }

        [SugarColumn(ColumnName = "status", ColumnDescription = "状态（0正常 1停用）")]
        public string Status { get; set; }

        [SugarColumn(ColumnName = "remark", ColumnDescription = "备注")]
        public string? Remark { get; set; }
    }
}