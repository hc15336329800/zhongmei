namespace RuoYi.Data
{
    public class DataConstants
    {
        public const string Master = "master";
        public const string Slave = "slave";
        public const string USER_NAME = "UserName";
        public const string USER_ID = "UserId";
        public const string USER_DEPT_ID = "DeptId";
        public const string LOGIN_USER_KEY = "";
    }

    public class DelFlag
    {
        public const string No = "0";
        public const string Yes = "2";
        public static string ToDesc(string? val)
        {
            return val switch
            {
                No => "未删除",
                Yes => "已删除",
                _ => "",
            };
        }
    }

    public class Status
    {
        public const string Enabled = "0";
        public const string Disabled = "1";
        public static string ToDesc(string? val)
        {
            return val switch
            {
                Enabled => "正常",
                Disabled => "停用",
                _ => "",
            };
        }

        public static string ToVal(string? desc)
        {
            return desc switch
            {
                "正常" => Enabled,
                "停用" => Disabled,
                _ => ""
            };
        }
    }

    public class Sex
    {
        public const string Male = "0";
        public const string Female = "1";
        public const string Unknown = "2";
        public static string ToDesc(string? val)
        {
            return val switch
            {
                Male => "男",
                Female => "女",
                _ => "未知"
            };
        }

        public static string ToVal(string? desc)
        {
            return desc switch
            {
                "男" => Male,
                "女" => Female,
                _ => Unknown
            };
        }
    }

    public class DataScope
    {
        public const string All = "1";
        public const string Custom = "2";
        public const string Department = "3";
        public const string DepartmentAndSub = "4";
        public static string ToDesc(string? val)
        {
            return val switch
            {
                All => "全部数据权限",
                Custom => "自定数据权限",
                Department => "本部门数据权限",
                DepartmentAndSub => "本部门及以下数据权限",
                _ => ""
            };
        }
    }

    public class YesNo
    {
        public const string No = "0";
        public const string Yes = "1";
        public static string ToDesc(string? val)
        {
            return val switch
            {
                No => "否",
                Yes => "是",
                "N" => "否",
                "Y" => "是",
                _ => ""
            };
        }

        public static string ToVal(string? desc)
        {
            return desc switch
            {
                "否" => No,
                "是" => Yes,
                _ => ""
            };
        }
    }
}