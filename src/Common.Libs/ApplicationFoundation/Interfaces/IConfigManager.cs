
namespace ApplicationFoundation.Interfaces
{
    public interface IConfigManager
    {
        T GetValue<T>(string parameter);
    }
}
