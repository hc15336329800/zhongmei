using RuoYi.Data;
using RuoYi.Data.Entities;
using RuoYi.Framework;
using RuoYi.Framework.Cache;
using RuoYi.Framework.Extensions;
using RuoYi.Framework.Utils;
using System.Text;

namespace RuoYi.Common.Utils
{
    public class DictUtils
    {
        public static string SEPARATOR = ",";
        public static void SetDictCache(string key, List<SysDictData> dictDatas)
        {
            App.GetService<ICache>().Set(GetCacheKey(key), dictDatas);
        }

        public static List<SysDictData> GetDictCache(string key)
        {
            return App.GetService<ICache>().Get<List<SysDictData>>(GetCacheKey(key));
        }

        public static string GetDictLabel(string dictType, string dictValue)
        {
            return GetDictLabel(dictType, dictValue, SEPARATOR);
        }

        public static string GetDictValue(string dictType, string dictLabel)
        {
            return GetDictValue(dictType, dictLabel, SEPARATOR);
        }

        public static string GetDictLabel(string dictType, string dictValue, string separator)
        {
            StringBuilder propertyString = new StringBuilder();
            List<SysDictData> datas = GetDictCache(dictType);
            if (datas != null)
            {
                if (StringUtils.ContainsAny(separator, dictValue))
                {
                    foreach (SysDictData dict in datas)
                    {
                        foreach (string value in dictValue.Split(separator))
                        {
                            if (value.Equals(dict.DictValue))
                            {
                                propertyString.Append(dict.DictLabel).Append(separator);
                                break;
                            }
                        }
                    }
                }
                else
                {
                    foreach (SysDictData dict in datas)
                    {
                        if (dictValue.Equals(dict.DictValue))
                        {
                            return dict.DictLabel!;
                        }
                    }
                }
            }

            return StringUtils.StripEnd(propertyString.ToString(), separator);
        }

        public static string GetDictValue(string dictType, string dictLabel, string separator)
        {
            StringBuilder propertyString = new StringBuilder();
            List<SysDictData> datas = GetDictCache(dictType);
            if (StringUtils.ContainsAny(separator, dictLabel) && datas.IsNotEmpty())
            {
                foreach (SysDictData dict in datas)
                {
                    foreach (string label in dictLabel.Split(separator))
                    {
                        if (label.Equals(dict.DictLabel))
                        {
                            propertyString.Append(dict.DictValue).Append(separator);
                            break;
                        }
                    }
                }
            }
            else
            {
                foreach (SysDictData dict in datas)
                {
                    if (dictLabel.Equals(dict.DictLabel))
                    {
                        return dict.DictValue!;
                    }
                }
            }

            return StringUtils.StripEnd(propertyString.ToString(), separator);
        }

        public static void RemoveDictCache(string key)
        {
            App.GetService<ICache>().Remove(GetCacheKey(key));
        }

        public static void ClearDictCache()
        {
            var redisCache = App.GetService<ICache>();
            redisCache.RemoveByPattern(CacheConstants.SYS_DICT_KEY + "*");
        }

        public static string GetCacheKey(string configKey)
        {
            return CacheConstants.SYS_DICT_KEY + configKey;
        }
    }
}