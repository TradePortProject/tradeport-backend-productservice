
using ProductManagement.Data;
using ProductManagement.Models;
using System;
using System.Threading.Tasks;

namespace ProductManagement.Repositories
{
    public interface IProductImageRepository: IRepositoryBase<ProductImage>
    {
        Task<ProductImage> AddProductImageAsync(ProductImage productImage);
       
        Task<List<ProductImage>> GetProductImageByIdAsync(Guid id);
    }
}
