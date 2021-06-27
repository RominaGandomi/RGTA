using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ConfigManager.Lib
{
    internal class Helper
    {
        private static readonly Dictionary<string, object> Cache = new Dictionary<string, object>();

        internal static T ConvertValue<T>(object val)
        {
            if (!(val is IConvertible)) throw new InvalidCastException("This object can not be converted");

            return (T)Convert.ChangeType(val, typeof(T));
        }

        public static object GetFromCache(string key)
        {
            return !Cache.ContainsKey(key) ? null : Cache[key];
        }

        public static void AddCache(string key, object value)
        {
            Cache[key] = value;
        }
    
    }
}
