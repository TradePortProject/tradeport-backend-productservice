
using System.Threading.Tasks;
using ProductManagement.Data;
using ProductManagement.Models;


namespace ProductManagement.Repositories
{
    public interface IProductRepository : IRepositoryBase<Product>
    {
        Task<List<Product>> GetAllProductsAsync();

        Task<Product?> GetProductByIdAsync(Guid Id);

        Task<Product> CreateProductAsync(Product product);

        Task<Product?> UpdateProductAsync(Guid Id, Product product);

        Task<string> GetProductCodeAsync();

    }
}
