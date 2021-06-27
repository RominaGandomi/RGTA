using ApplicationFoundation;
using ApplicationFoundation.DiResolvers;
using ApplicationFoundation.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ConfigManager.Lib
{
    internal class FileConfigReader : ConfigReader
    {
        private static IConfigurationRoot _configuration;
        public static IConfiguration Configuration
        {
            get
            {
                if (_configuration != null) return _configuration;
                GetConfigFromFile();
                return _configuration;
            }
            set => _configuration = (IConfigurationRoot)value;
        }
        public static void GetConfigFromFile()
        {
            try
            {
                var builder = new ConfigurationBuilder();
                var fullPath = new StringBuilder();
                var projName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;

                fullPath.Append(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent?.Parent?.Parent?.Parent?.Parent?.Parent)
                        .Append("\\").Append("Common.Libs\\").Append(projName).Append("\\");

                builder.SetBasePath(fullPath.ToString()).AddJsonFile($"appsettings.Development.json", optional: true, reloadOnChange: true);
                Configuration = builder.Build();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        protected override object GetConfigValue(string parameter)
        {
            var valueToReturn = Configuration.GetSection(parameter).Value;
            return valueToReturn;
        }
       
    }

}
