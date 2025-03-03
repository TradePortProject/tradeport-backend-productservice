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

        private readonly AppDbContext dbContext;
        public ProductImageRepository(AppDbContext dbContextRepo) : base(dbContextRepo)
        {
            this.dbContext = dbContextRepo;
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


        public async Task<List<ProductImage>> GetProductImageByIdAsync(Guid id)
        {
            return await FindByCondition(productmage => productmage.ProductID== id).ToListAsync();
        }
    }
}
