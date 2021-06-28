using ApplicationFoundation.DiResolvers;
using ApplicationFoundation.Interfaces;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ServiceBusManager.Lib.Helpers
{
    public class Extension
    {
        public static T ChangeMessageType<T>(object message) where T : class
        {
            if (message is IConvertible) return (T)Convert.ChangeType(message, typeof(T));
            var serializedData = Serialize(message);
            var deserializedData = Deserialize<T>(serializedData);
            return deserializedData;
        }
        public static byte[] Serialize(object obj)
        {
            if (obj == null)
                return null;
            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }
        public static T Deserialize<T>(byte[] data)
        {
            if (data == null)
                return default;
            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream(data))
            {
                var obj = bf.Deserialize(ms);
                return (T)obj;
            }
        }
    }
    public static class ConfigExtension
    {
        public static IConfigManager Instance
        {
            get
            {
                return DependencyResolver.Instance.Container.Resolve<IConfigManager>();
            }
        }
    }
}
