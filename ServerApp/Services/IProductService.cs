namespace ServerApp.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetProductsAsync();
    }

    public class ProductDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public double Price { get; set; }
        public int Stock { get; set; }
        public CategoryDto? Category { get; set; }
    }

    public class CategoryDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }
}
