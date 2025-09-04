namespace RuoYi.Data
{
    public class Constants
    {
        public const string UTF8 = "UTF-8";
        public const string GBK = "GBK";
        public const string WWW = "www.";
        public const string HTTP = "http://";
        public const string HTTPS = "https://";
        public const string SUCCESS = "0";
        public const string FAIL = "1";
        public const string LOGIN_SUCCESS = "Success";
        public const string LOGOUT = "Logout";
        public const string REGISTER = "Register";
        public const string LOGIN_FAIL = "Error";
        public const int CAPTCHA_EXPIRATION = 2;
        public const string TOKEN = "token";
        public const string TOKEN_PREFIX = "Bearer ";
        public const string LOGIN_USER_KEY = "login_user_key";
        public const string JWT_USERID = "userid";
        public const string JWT_USERNAME = "sub";
        public const string JWT_AVATAR = "avatar";
        public const string JWT_CREATED = "created";
        public const string JWT_AUTHORITIES = "authorities";
        public const string RESOURCE_PREFIX = "/profile";
        public const string LOOKUP_RMI = "rmi:";
        public const string LOOKUP_LDAP = "ldap:";
        public const string LOOKUP_LDAPS = "ldaps:";
        public static string[] JSON_WHITELIST_STR =
        {
            "org.springframework",
            "com.ruoyi"
        };
        public static string[] JOB_WHITELIST_STR =
        {
            "com.ruoyi"
        };
        public static string[] JOB_ERROR_STR =
        {
            "java.net.URL",
            "javax.naming.InitialContext",
            "org.yaml.snakeyaml",
            "org.springframework",
            "org.apache",
            "com.ruoyi.common.utils.file",
            "com.ruoyi.common.config"
        };
    }
}