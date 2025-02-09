
using ProductManagement.Models;


namespace ProductManagement.Repositories
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllProdcutsAsync();

        Task<Product?> GetProductByIdAsync(Guid Id);

        Task<Product> CreateProductAsync(Product product);

        Task<Product?> UpdateProductAsync(Guid Id, Product product);

        Task<Product?> DeleteAysnc(Guid Id);

    }
}
