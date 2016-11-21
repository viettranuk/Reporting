using System.Runtime.Caching;

namespace Report.Services
{
    public interface ICacheWrapper
    {
        object Get(string key);
        object AddOrGetExisting(string key, object value);
    }
}
