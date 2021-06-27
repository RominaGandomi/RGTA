
namespace ConfigManager.Lib
{
    internal abstract class ConfigReader
    {
        protected abstract object GetConfigValue(string parameter);
        public virtual object GetValue(string parameter)
        {
            var val = Helper.GetFromCache(parameter);
            if (val != null)
                return val;
            val = GetConfigValue(parameter);
            Helper.AddCache(parameter, val);
            return val;
        }
    }
}
