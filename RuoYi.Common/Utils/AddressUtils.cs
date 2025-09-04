using Newtonsoft.Json.Linq;
using RuoYi.Framework;
using RuoYi.Framework.Logging;
using RuoYi.Framework.RemoteRequest.Extensions;
using RuoYi.Framework.Utils;

namespace RuoYi.Common.Utils
{
    public static class AddressUtils
    {
        public static string IP_URL = "http://whois.pconline.com.cn/ipJson.jsp";
        public static string UNKNOWN = "X.X.X.X";
        public static async Task<string> GetRealAddressByIPAsync(string ip)
        {
            if (IpUtils.IsInternalIp(ip))
            {
                return "内网IP";
            }

            var ruoYiConfig = RyApp.RuoYiConfig;
            if (ruoYiConfig.AddressEnabled)
            {
                try
                {
                    var url = $"{IP_URL}?ip={ip}&json=true";
                    var rspStr = await url.GetAsStringAsync();
                    if (string.IsNullOrEmpty(rspStr))
                    {
                        Log.Error("获取地理位置异常 {}", ip);
                        return UNKNOWN;
                    }

                    JObject obj = JObject.Parse(rspStr);
                    string region = obj.GetValue("pro")?.ToString();
                    string city = obj.GetValue("city")?.ToString();
                    return $"{region} {city}";
                }
                catch (Exception e)
                {
                    Log.Error("获取地理位置异常 {}-{}", ip, e.Message);
                }
            }

            return UNKNOWN;
        }
    }
}