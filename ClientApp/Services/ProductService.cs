using System.Text.Json;

namespace ClientApp.Services
{
    public interface IProductService
    {
        Task<Product[]?> GetProductsAsync();
        Task ClearCacheAsync();
    }

    public class ProductService : IProductService
    {
        private readonly HttpClient _http;
        private readonly IStorageService _storage;
        private const string CacheKey = "products_cache";
        private const string ApiUrl = "http://localhost:5242/api/productlist";
        private const int CacheDurationMinutes = 30;

        public ProductService(HttpClient http, IStorageService storage)
        {
            _http = http;
            _storage = storage;
        }

        public async Task<Product[]?> GetProductsAsync()
        {
            // Intentar obtener del caché local
            var cachedProducts = await _storage.GetAsync<Product[]>(CacheKey);
            if (cachedProducts != null)
            {
                System.Diagnostics.Debug.WriteLine("Products loaded from local cache");
                return cachedProducts;
            }

            try
            {
                // Si no está en caché, hacer llamada al API
                using var cts = new System.Threading.CancellationTokenSource(TimeSpan.FromSeconds(10));
                var response = await _http.GetAsync(ApiUrl, cts.Token);
                response.EnsureSuccessStatusCode();
                
                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var products = JsonSerializer.Deserialize<Product[]>(json, options);

                // Guardar en caché
                if (products != null)
                {
                    await _storage.SetAsync(CacheKey, products, TimeSpan.FromMinutes(CacheDurationMinutes));
                    System.Diagnostics.Debug.WriteLine($"Products cached for {CacheDurationMinutes} minutes");
                }

                return products;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching products: {ex.Message}");
                throw;
            }
        }

        public async Task ClearCacheAsync()
        {
            await _storage.RemoveAsync(CacheKey);
            System.Diagnostics.Debug.WriteLine("Product cache cleared");
        }
    }

    public class Product
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public double Price { get; set; }
        public int Stock { get; set; }
        public Category? Category { get; set; }
    }

    public class Category
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }
}
