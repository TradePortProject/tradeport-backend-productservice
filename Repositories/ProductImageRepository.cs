using Microsoft.EntityFrameworkCore;
using ProductManagement.Models;
using ProductManagement.Data;
using System;
using System.Threading.Tasks;
//using PaymentGateway.Data;

namespace ProductManagement.Repositories
{
    public class ProductImageRepository : RepositoryBase<ProductImage>, IProductImageRepository
    {
        public ProductImageRepository(AppDbContext dbContextRepo) : base(dbContextRepo)
        {
        }

        public async Task<ProductImage> AddProductImageAsync(ProductImage productImage)
        {
            try
            {
                await CreateAsync(productImage); // Using updated async method
                await SaveAsync(); // Using updated async save method
                return productImage;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while saving product image", ex);
            }
        }
    }
}
