using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductManagement.Data;
using ProductManagement.Models;
using ProductManagement.Models.DTO;
using ProductManagement.Repositories;
using ProductManagement.Utilities;


namespace ProductManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductMangementController : ControllerBase
    {

        private readonly AppDbContext dbContext;
        private readonly IProductRepository productRepository;
        public ProductMangementController(AppDbContext appDbContext, IProductRepository prodRepo)
        {
            this.dbContext = appDbContext;
            this.productRepository = prodRepo;

        }     


        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            try
            {
                var productsModel = await productRepository.GetAllProdcutsAsync();

                if (productsModel == null || !productsModel.Any())
                {
                    return NotFound(new
                    {
                        Message = "No products found.",                       
                        ErrorMessage = "No data available."
                    });
                }

                // Map database entities to DTOs
                var productDTOs = productsModel.Select(product => new ProductDTO
                {
                    ProductID = product.ProductID,
                    ProductCode = product.ProductCode,
                    ManufacturerID = product.ManufacturerID,
                    ProductName = product.ProductName,
                    Description = product.Description,
                    CategoryDescription = EnumHelper.GetDescription<Category>(product.Category),
                    WholesalePrice = product.WholesalePrice,
                    RetailPrice = product.RetailPrice,
                    Quantity = product.Quantity,
                    RetailCurrency = product.RetailCurrency,
                    WholeSaleCurrency = product.WholeSaleCurrency,
                    ShippingCost = product.ShippingCost,
                    CreatedOn = product.CreatedOn,
                    UpdatedOn = product.UpdatedOn,
                    IsActive = product.IsActive
                }).ToList();



            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "An error occurred while retrieving the products.",                   
                    ErrorMessage = ex.Message
                });
            }
        }


        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetProductById(Guid id)
        {
            try
            {
                var productById = await productRepository.GetProductByIdAsync(id);

                if (productById == null)
                {
                    return NotFound(new
                    {
                        Message = "Product not found.",
                        ProductCode = string.Empty,
                        ErrorMessage = "Invalid product ID."
                    });
                }

                // Map the database entity to DTO
                var productDto = new ProductDTO()
                {
                    ProductID = productById.ProductID,
                    ProductCode = productById.ProductCode,
                    ManufacturerID = productById.ManufacturerID,
                    ProductName = productById.ProductName,
                    Description = productById.Description,
                    CategoryDescription = EnumHelper.GetDescription<Category>(prodcutById.Category),
                    WholesalePrice = productById.WholesalePrice,
                    RetailPrice = productById.RetailPrice,
                    Quantity = productById.Quantity,
                    RetailCurrency = productById.RetailCurrency,
                    WholeSaleCurrency = productById.WholeSaleCurrency,
                    ShippingCost = productById.ShippingCost,
                    CreatedOn = productById.CreatedOn,
                    UpdatedOn = productById.UpdatedOn,
                    IsActive = productById.IsActive
                };

                return Ok(new
                {
                    Message = "Product retrieved successfully.",
                    Product = productDto,
                    ErrorMessage = string.Empty
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "An error occurred while retrieving the product.",
                    ProductCode = string.Empty,
                    ErrorMessage = ex.Message
                });
            }
        }

             

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDTO addProductRequestDto)
        {
            var productModel = new Product()
            {
                ProductCode = addProductRequestDto.ProductCode,
                //ManufacturerID = addProductRequestDto.ManufacturerID,
                ProductName = addProductRequestDto.ProductName,
                Description = addProductRequestDto.Description,
                //Category = addProductRequestDto.Category,
                Category = EnumHelper.GetEnumFromDescription<Category>(addProductRequestDto.CategoryDescription),
                WholesalePrice = addProductRequestDto.WholesalePrice,
                RetailPrice = addProductRequestDto.RetailPrice,
                Quantity = addProductRequestDto.Quantity,
                RetailCurrency = addProductRequestDto.RetailCurrency,
                WholeSaleCurrency = addProductRequestDto.WholeSaleCurrency,
                ShippingCost = addProductRequestDto.ShippingCost,
                CreatedOn = addProductRequestDto.CreatedOn,
                UpdatedOn = addProductRequestDto.UpdatedOn,
                IsActive = addProductRequestDto.IsActive

            };

                // Use Repository to create Product
                productModel = await productRepository.CreateProductAsync(productModel);

                var response = new
                {
                    Message = "Product created successfully.",
                    ProductCode = productModel.ProductCode,
                    ErrorMessage = ""
                };

                return CreatedAtAction(nameof(GetProductById), new { id = productModel.ProductID }, response);
            }
            catch (Exception ex)
            {
                var response = new
                {
                    Message = "Product creation failed.",
                    ProductCode = string.Empty,
                    ErrorMessage = ex.Message
                };

                return StatusCode(500, response);
            }
        }


        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] UpdateProductDTO updateProductRequestDto)
        {
            try
            {
                // Check if the product exists
                var existingProduct = await productRepository.GetProductByIdAsync(id);
                if (existingProduct == null)
                {
                    return NotFound(new
                    {
                        Message = "Product not found.",
                        ProductCode = string.Empty,
                        ErrorMessage = "Invalid product ID."
                    });
                }

                // Update properties
                existingProduct.ProductCode = updateProductRequestDto.ProductCode;                
                existingProduct.ProductName = updateProductRequestDto.ProductName;
                existingProduct.Description = updateProductRequestDto.Description;
                Category = EnumHelper.GetEnumFromDescription<Category>(updateProductRequestDto.CategoryDescription),
                existingProduct.WholesalePrice = updateProductRequestDto.WholesalePrice;
                existingProduct.RetailPrice = updateProductRequestDto.RetailPrice;
                existingProduct.Quantity = updateProductRequestDto.Quantity;
                existingProduct.RetailCurrency = updateProductRequestDto.RetailCurrency;
                existingProduct.WholeSaleCurrency = updateProductRequestDto.WholeSaleCurrency;
                existingProduct.ShippingCost = updateProductRequestDto.ShippingCost;
                existingProduct.CreatedOn = updateProductRequestDto.CreatedOn;
                existingProduct.UpdatedOn = DateTime.UtcNow;
                existingProduct.IsActive = updateProductRequestDto.IsActive;

                // Update the product in the repository
                var updatedProduct = await productRepository.UpdateProductAsync(id, existingProduct);

                if (updatedProduct == null)
                {
                    return StatusCode(500, new
                    {
                        Message = "Failed to update product.",
                        ProductCode = string.Empty,
                        ErrorMessage = "Internal server error."
                    });
                }


            ////Map the product Model back to DTO
            //var productDto = new ProductDTO()
            //{
            //    ProductCode = productModel.ProductCode,
            //    // ManufacturerID = productModel.ManufacturerID,
            //    ProductName = productModel.ProductName,
            //    Description = productModel.Description,
            //    //Category = productModel.Category,
            //    WholesalePrice = productModel.WholesalePrice,
            //    RetailPrice = productModel.RetailPrice,
            //    Quantity = productModel.Quantity,
            //    RetailCurrency = productModel.RetailCurrency,
            //    WholeSaleCurrency = productModel.WholeSaleCurrency,  
            //    ShippingCost = productModel.ShippingCost,
            //    CreatedOn = productModel.CreatedOn,
            //    UpdatedOn = productModel.UpdatedOn,
            //    IsActive = productModel.IsActive

            //};


            return Ok("Record updated successfully");
                return Ok(new
                {
                    Message = "Product updated successfully.",
                    ProductCode = updatedProduct.ProductCode,
                    ErrorMessage = string.Empty
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "An error occurred while updating the product.",
                    ProductCode = string.Empty,
                    ErrorMessage = ex.Message
                });
            }
        }


        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            try
            {
                // Check if the product exists
                var productById = await productRepository.GetProductByIdAsync(id);
                if (productById == null)
                {
                    return NotFound(new
                    {
                        Message = "Product not found.",
                        ProductCode = string.Empty,
                        ErrorMessage = "Invalid product ID."
                    });
                }

                // Delete the product from repository
                var deletedProduct = await productRepository.DeleteAysnc(id);

            product.IsActive = false;

            await productRepository.UpdateProductAsync(id, product);
                return Ok(new
                {
                    Message = "Product deleted successfully.",
                    ProductCode =  deletedProduct.ProductCode,
                    ErrorMessage = string.Empty
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "An error occurred while deleting the product.",
                    ProductCode = string.Empty,
                    ErrorMessage = ex.Message
                });
            }
        }


    }
}

