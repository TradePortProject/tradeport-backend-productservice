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
                var productsModel = await productRepository.GetAllProductsAsync();

                if (productsModel == null || !productsModel.Any())
                {
                    return NotFound(new
                    {
                        Message = "No products found.",
                        ErrorMessage = "No data available."
                    });
                }

                // Map database entities to DTOs
                //var productDTOs = productsModel.Select(product => new ProductDTO
                //{
                //    ProductID = product.ProductID,
                //    ProductCode = product.ProductCode,
                //    ManufacturerID = product.ManufacturerID,
                //    ProductName = product.ProductName,
                //    Description = product.Description,
                //    CategoryDescription = EnumHelper.GetDescription<Category>(product.Category),
                //    WholesalePrice = product.WholesalePrice,
                //    RetailPrice = product.RetailPrice,
                //    Quantity = product.Quantity,
                //    RetailCurrency = product.RetailCurrency,
                //    WholeSaleCurrency = product.WholeSaleCurrency,
                //    ShippingCost = product.ShippingCost,
                //    CreatedOn = product.CreatedOn,
                //    UpdatedOn = product.UpdatedOn,
                //    IsActive = product.IsActive
                //}).ToList();

                // Use AutoMapper to map the list of Product entities to ProductDTOs.
                var productDTOs = _mapper.Map<List<ProductDTO>>(productsModel);

                return Ok(new
                {
                    Message = "Products retrieved successfully.",
                    Products = productDTOs,
                    ErrorMessage = string.Empty
                });
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
                //var productDto = new ProductDTO()
                //{
                //    ProductID = productById.ProductID,
                //    ProductCode = productById.ProductCode,
                //    ManufacturerID = productById.ManufacturerID,
                //    ProductName = productById.ProductName,
                //    Description = productById.Description,
                //    CategoryDescription = EnumHelper.GetDescription<Category>(productById.Category),
                //    WholesalePrice = productById.WholesalePrice,
                //    RetailPrice = productById.RetailPrice,
                //    Quantity = productById.Quantity,
                //    RetailCurrency = productById.RetailCurrency,
                //    WholeSaleCurrency = productById.WholeSaleCurrency,
                //    ShippingCost = productById.ShippingCost,
                //    CreatedOn = productById.CreatedOn,
                //    UpdatedOn = productById.UpdatedOn,
                //    IsActive = productById.IsActive
                //};

                // Use AutoMapper to map the Product entity to ProductDTO.
                var productDto = _mapper.Map<ProductDTO>(productById);

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
            try
            {
                var productModel = new Product()
                {
                    ManufacturerID = addProductRequestDto.ManufacturerID,
                    ProductName = addProductRequestDto.ProductName,
                    Description = addProductRequestDto.Description,
                    Category = EnumHelper.GetEnumFromDescription<Category>(addProductRequestDto.CategoryDescription),
                    WholesalePrice = addProductRequestDto.WholesalePrice,
                    RetailPrice = addProductRequestDto.RetailPrice,
                    Quantity = addProductRequestDto.Quantity,
                    RetailCurrency = addProductRequestDto.RetailCurrency,
                    WholeSaleCurrency = addProductRequestDto.WholeSaleCurrency,
                    ShippingCost = addProductRequestDto.ShippingCost,
                    CreatedOn = DateTime.UtcNow,
                    UpdatedOn = DateTime.UtcNow,
                    IsActive = true
                //var productModel = new Product()
                //{
                //    ProductCode = addProductRequestDto.ProductCode,
                //    //ManufacturerID = addProductRequestDto.ManufacturerID,
                //    ProductName = addProductRequestDto.ProductName,
                //    Description = addProductRequestDto.Description,
                //    //Category = addProductRequestDto.Category,
                //    Category = EnumHelper.GetEnumFromDescription<Category>(addProductRequestDto.CategoryDescription),
                //    WholesalePrice = addProductRequestDto.WholesalePrice,
                //    RetailPrice = addProductRequestDto.RetailPrice,
                //    Quantity = addProductRequestDto.Quantity,
                //    RetailCurrency = addProductRequestDto.RetailCurrency,
                //    WholeSaleCurrency = addProductRequestDto.WholeSaleCurrency,
                //    ShippingCost = addProductRequestDto.ShippingCost,
                //    CreatedOn = addProductRequestDto.CreatedOn,
                //    UpdatedOn = addProductRequestDto.UpdatedOn,
                //    IsActive = addProductRequestDto.IsActive

                //};

                // Map the incoming CreateProductDTO to a Product entity.
                var productModel = _mapper.Map<Product>(addProductRequestDto);

                // Use Repository to create Product
                productModel = await productRepository.CreateProductAsync(productModel);

                // (Optional) Map back to a ProductDTO if you want to return the created product details.
                var productDto = _mapper.Map<ProductDTO>(productModel);

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
                //existingProduct.ProductCode = updateProductRequestDto.ProductCode;
                //existingProduct.ProductName = updateProductRequestDto.ProductName;
                //existingProduct.Description = updateProductRequestDto.Description;
                //existingProduct.Category = EnumHelper.GetEnumFromDescription<Category>(updateProductRequestDto.CategoryDescription);
                //existingProduct.WholesalePrice = updateProductRequestDto.WholesalePrice;
                //existingProduct.RetailPrice = updateProductRequestDto.RetailPrice;
                //existingProduct.Quantity = updateProductRequestDto.Quantity;
                //existingProduct.RetailCurrency = updateProductRequestDto.RetailCurrency;
                //existingProduct.WholeSaleCurrency = updateProductRequestDto.WholeSaleCurrency;
                //existingProduct.ShippingCost = updateProductRequestDto.ShippingCost;
                //existingProduct.CreatedOn = updateProductRequestDto.CreatedOn;
                //existingProduct.UpdatedOn = DateTime.UtcNow;
                //existingProduct.IsActive = updateProductRequestDto.IsActive;

                // Use AutoMapper to update the existing product with values from the DTO.
                _mapper.Map(updateProductRequestDto, existingProduct);

                // Optionally update properties that aren’t handled by AutoMapper.
                existingProduct.UpdatedOn = DateTime.UtcNow;
                //existingProduct.IsActive = updateProductRequestDto.IsActive;

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

                productById.IsActive = false;

                await productRepository.UpdateProductAsync(id, productById);
                return Ok(new
                {
                    Message = "Product deleted successfully.",
                    ProductCode = productById.ProductCode,
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

