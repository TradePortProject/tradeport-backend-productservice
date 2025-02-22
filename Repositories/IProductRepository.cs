
using System.Threading.Tasks;
using ProductManagement.Data;
using ProductManagement.Models;


namespace ProductManagement.Repositories
{
    public interface IProductRepository : IRepositoryBase<Product>
    {
        Task<List<Product>> GetAllProductsAsync();

        Task<List<Product>> GetFilteredProductsAsync(
        string? searchText,
        int? category,
        decimal? minWholesalePrice,
        decimal? maxWholesalePrice,
        decimal? minRetailPrice,
        decimal? maxRetailPrice,
        int? quantity,
        string? sortBy,
        bool? sortDescending,
        int? pageNumber,
        int? pageSize);

        Task<Product?> GetProductByIdAsync(Guid Id);

        Task<Product> CreateProductAsync(Product product);

        Task<Product?> UpdateProductAsync(Guid Id, Product product);

        Task<string> GetProductCodeAsync();

        Task InsertProductImageAsync(ProductImage productImage);

    }
}
