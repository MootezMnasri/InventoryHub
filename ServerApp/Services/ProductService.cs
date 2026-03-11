using Microsoft.Extensions.Caching.Memory;

namespace ServerApp.Services
{
    public class ProductService : IProductService
    {
        private readonly IMemoryCache _cache;
        private const string CacheKey = "ProductList";
        private const int CacheDurationMinutes = 30;

        public ProductService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public async Task<IEnumerable<ProductDto>> GetProductsAsync()
        {
            // Intentar obtener del caché
            if (_cache.TryGetValue(CacheKey, out IEnumerable<ProductDto>? cachedProducts))
            {
                return cachedProducts!;
            }

            // Si no está en caché, crear la lista
            var products = new[]
            {
                new ProductDto
                {
                    Id = 1,
                    Name = "Laptop",
                    Price = 1200.50,
                    Stock = 25,
                    Category = new CategoryDto { Id = 101, Name = "Electronics" }
                },
                new ProductDto
                {
                    Id = 2,
                    Name = "Headphones",
                    Price = 50.00,
                    Stock = 100,
                    Category = new CategoryDto { Id = 102, Name = "Accessories" }
                }
            };

            // Guardar en caché
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheDurationMinutes));

            _cache.Set(CacheKey, products, cacheOptions);

            return await Task.FromResult(products);
        }
    }
}
