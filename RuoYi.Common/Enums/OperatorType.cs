using System.ComponentModel;

namespace RuoYi.Common.Enums
{
    public enum OperatorType
    {
        [Description("其它")]
        OTHER,
        [Description("后台用户")]
        MANAGE,
        [Description("手机端用户")]
        MOBILE
    }
}