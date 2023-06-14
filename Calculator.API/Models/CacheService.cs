using System.Runtime.Caching;

namespace Calculator.API.Models
{
    public interface ICacheService
    {
        T? Get<T>(string key);
        bool Set<T>(string key, T value);
        bool Delete<T>(string key);
    }

    public class CacheService : ICacheService
    {
        private readonly ObjectCache _memoryCache = MemoryCache.Default;
        private readonly ILogger<CacheService> _logger;

        public CacheService(ILogger<CacheService> logger)
        {
            _logger = logger;
        }
      

        public T? Get<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            try
            {
                T? item = (T)_memoryCache.Get(key);
                return item;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "When Getting from Cache - key {0}", key);
                return default;
            }
        }

        public bool Set<T>(string key, T value)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));
            if (value == null) throw 
                new ArgumentNullException(nameof(value));

            try
            {
                _memoryCache.Set(key, value, new CacheItemPolicy());
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "When Setting value in Cache - key {0}, value {1}", key, value);
                return false;
            }
            
        }

        public bool Delete<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            try
            {
                _memoryCache.Remove(key);
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "When Deleting from Cache - key {0}", key); 
                return false; 

            }
        }
    }
}
