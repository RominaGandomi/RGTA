using ApplicationFoundation.Interfaces;
using System;
using System.Configuration;

namespace ConfigManager.Lib
{
    public class ConfigManager : IConfigManager
    {
        private static ConfigReader _reader;

        public T GetValue<T>(string parameter)
        {
            _reader = GetConfigReader();
            var val = _reader.GetValue(parameter);
            return Helper.ConvertValue<T>(val);
        }
        private static ConfigReader GetConfigReader()
        {
            if (_reader != null) return _reader;
            var typeName = "ConfigManager.Lib.FileConfigReader";
            var readerType = Type.GetType(typeName);
            if (readerType != null) _reader = (ConfigReader)Activator.CreateInstance(readerType);
            return _reader;

        }
    }
}
