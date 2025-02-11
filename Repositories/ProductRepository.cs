using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PaymentGateway.Data;
using ProductManagement.Data;
using ProductManagement.Models;


namespace ProductManagement.Repositories
{
    public class ProductRepository : RepositoryBase<Product>, IProductRepository
    {
        private readonly AppDbContext dbContext;
        public ProductRepository(AppDbContext dbContextRepo) : base(dbContextRepo)
        {
            this.dbContext = dbContextRepo;
        }



        public async Task<List<Product>> GetAllProdcutsAsync()
        {
            return await FindByCondition(product=> product.IsActive).OrderBy(x => x.CreatedOn).ToListAsync();
            //   return await dbContext.Products.ToListAsync();
        }

        public async Task<Product> GetProductByIdAsync(Guid Id)
        {
            return await FindByCondition(product => product.IsActive).OrderBy(x => x.ProductCode).FirstOrDefaultAsync();
            //return await dbContext.Products.FindAsync(Id);
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            await dbContext.Products.AddAsync(product);
            await dbContext.SaveChangesAsync();
            return product;
        }

        public async Task<Product?> UpdateProductAsync(Guid id, Product product)
        {
            var prodcutById = await dbContext.Products.FindAsync(id);

            if (prodcutById == null)
            {
                return null;
            }

            prodcutById.ProductCode = product.ProductCode;
            //prodcutById.ManufacturerID = product.ManufacturerID;
            prodcutById.ProductName = product.ProductName;
            prodcutById.Description = product.Description;
            prodcutById.Category = product.Category;
            prodcutById.WholesalePrice = product.WholesalePrice;
            prodcutById.RetailPrice = product.RetailPrice;
            prodcutById.Quantity = product.Quantity;
            prodcutById.RetailCurrency = product.RetailCurrency;
            prodcutById.WholeSaleCurrency = product.WholeSaleCurrency;
            prodcutById.ShippingCost = product.ShippingCost;
            prodcutById.CreatedOn = product.CreatedOn;
            prodcutById.UpdatedOn = product.UpdatedOn;
            prodcutById.IsActive = product.IsActive;

            await dbContext.SaveChangesAsync();
            return prodcutById;
        }

        public async Task<Product?> DeleteAysnc(Guid Id)
        {
            var prodcutById = await dbContext.Products.FindAsync(Id);

            if (prodcutById == null)
            {
                return null;

            }

            dbContext.Products.Remove(prodcutById);
            await dbContext.SaveChangesAsync();
            return prodcutById;
        }

    }
}
