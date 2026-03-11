using System.Text.Json;

namespace ClientApp.Services
{
    public interface IStorageService
    {
        Task<T?> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
        Task RemoveAsync(string key);
    }

    public class LocalStorageService : IStorageService
    {
        private readonly Dictionary<string, (object Value, DateTime ExpirationTime)> _storage = new();

        public Task<T?> GetAsync<T>(string key)
        {
            if (_storage.TryGetValue(key, out var item))
            {
                // Verificar si ha expirado
                if (DateTime.UtcNow < item.ExpirationTime)
                {
                    return Task.FromResult((T?)item.Value);
                }
                else
                {
                    // Eliminar si ha expirado
                    _storage.Remove(key);
                }
            }
            return Task.FromResult<T?>(default);
        }

        public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            var expirationTime = expiration.HasValue 
                ? DateTime.UtcNow.Add(expiration.Value) 
                : DateTime.UtcNow.AddHours(1);

            _storage[key] = (value!, expirationTime);
            return Task.CompletedTask;
        }

        public Task RemoveAsync(string key)
        {
            _storage.Remove(key);
            return Task.CompletedTask;
        }
    }
}
