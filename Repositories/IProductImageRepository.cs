
using ProductManagement.Models;
using System;
using System.Threading.Tasks;

namespace ProductManagement.Repositories
{
    public interface IProductImageRepository
    {
        Task<ProductImage> AddProductImageAsync(ProductImage productImage);
    }
}
