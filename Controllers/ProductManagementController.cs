using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductManagement.Data;
using ProductManagement.Models;
using ProductManagement.Models.DTO;
using ProductManagement.Repositories;
using AutoMapper;
using ProductManagement.Utilities;
using Microsoft.AspNetCore.Authorization;


namespace ProductManagement.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    
    public class ProductManagementController : ControllerBase
    {

        private readonly AppDbContext dbContext;
        private readonly IProductRepository productRepository;
        private readonly IProductImageRepository productImageRepository;
        private readonly IMapper _mapper;
        public ProductManagementController(AppDbContext appDbContext, IProductRepository prodRepo, IProductImageRepository productImageRepo, IMapper mapper)
        {
            this.dbContext = appDbContext;
            this.productRepository = prodRepo;
            this.productImageRepository = productImageRepo;
            _mapper = mapper;
        }

        [Authorize]
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

                // Get related Product Image details for each product
                var productImage = await productImageRepository.FindByCondition(prd => productsModel.Select(p => p.ProductID).Contains(prd.ProductID)).ToListAsync();

                // Use AutoMapper to map the list of Product entities to ProductDTOs.
                var productDTOs = _mapper.Map<List<ProductDTO>>(productsModel);
                var productImageDto = _mapper.Map<List<ProductImageDTO>>(productImage);


                return Ok(new
                {

                    Message = "Product retrieved successfully.",
                    ErrorMessage = string.Empty,
                    Product = productDTOs.Select(prod => new
                    {
                        ProductID = prod.ProductID,
                        ProductCode = prod.ProductCode,
                        ManufacturerID = prod.ManufacturerID,
                        ProductName = prod.ProductName,
                        Description = prod.Description,
                        Category = prod.Category,
                        WholesalePrice = prod.WholesalePrice,
                        RetailPrice = prod.RetailPrice,
                        Quantity = prod.Quantity,
                        RetailCurrency = prod.RetailCurrency,
                        WholeSaleCurrency = prod.WholeSaleCurrency,
                        ShippingCost = prod.ShippingCost,
                        CreatedOn = prod.CreatedOn,
                        UpdatedOn = prod.UpdatedOn,
                        IsActive = prod.IsActive,

                        ProductImage = productImageDto
                                        .Where(imageDetails => imageDetails.ProductID == prod.ProductID) // Filter images by ProductID
                                        .Select(imageDetails => new
                                        {
                                            ProductImageURL = imageDetails.ProductImageURL
                                        }).ToList()
                    }).ToList()
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

        [HttpGet("GetFilteredProducts")]
        public async Task<IActionResult> GetFilteredProducts(
        [FromQuery] string? searchText = null,
        [FromQuery] int? category = null,
        [FromQuery] decimal? minWholesalePrice = null,
        [FromQuery] decimal? maxWholesalePrice = null,
        [FromQuery] decimal? minRetailPrice = null,
        [FromQuery] decimal? maxRetailPrice = null,
        [FromQuery] int? quantity = null,
        [FromQuery] bool? sortDescending = null,
        [FromQuery] int? pageNumber = null,
        [FromQuery] int? pageSize = null)
        {
            try
            {
                int pageNumberValue = pageNumber ?? 1;
                int pageSizeValue = pageSize ?? 10;

                var products = await productRepository.GetFilteredProductsAsync(
                    searchText, category, minWholesalePrice, maxWholesalePrice, minRetailPrice,
                    maxRetailPrice, quantity, sortDescending, pageNumberValue, pageSizeValue);

                if (!products.Any())
                {
                    return NotFound(new { Message = "No results found. Please adjust your filters.", ErrorMessage = "No data available." });
                }

                // Get related Product Image details for each product
                var productImage = await productImageRepository.FindByCondition(prd => products.Select(p => p.ProductID).Contains(prd.ProductID)).ToListAsync();

                int totalProducts = await productRepository.GetTotalProductCountAsync(searchText, category, minWholesalePrice, maxWholesalePrice, minRetailPrice, maxRetailPrice, quantity);
                int totalPages = (int)Math.Ceiling((double)totalProducts / pageSizeValue);

                // Use AutoMapper to map the list of Product entities to ProductDTOs.
                var productDTOs = _mapper.Map<List<ProductDTO>>(products);
                var productImageDto = _mapper.Map<List<ProductImageDTO>>(productImage);

                return Ok(new
                {
                    //Message = "Products retrieved successfully.",
                    //Product = productDTOs,
                    //TotalPages = totalPages,
                    //SearchText = searchText ?? "",
                    //Category = category,
                    //MinWholesalePrice = minWholesalePrice,
                    //MaxWholesalePrice = maxWholesalePrice,
                    //MinRetailPrice = minRetailPrice,
                    //MaxRetailPrice = maxRetailPrice,
                    //Quantity = quantity,
                    //SortDescending = sortDescending ?? false,
                    //PageNumber = pageNumberValue,
                    //PageSize = pageSizeValue

                    Message = "Product retrieved successfully.",
                    ErrorMessage = string.Empty,
                    Product = productDTOs.Select(prod => new
                    {
                        ProductID = prod.ProductID,
                        ProductCode = prod.ProductCode,
                        ManufacturerID = prod.ManufacturerID,
                        ProductName = prod.ProductName,
                        Description = prod.Description,
                        Category = prod.Category,
                        WholesalePrice = prod.WholesalePrice,
                        RetailPrice = prod.RetailPrice,
                        Quantity = prod.Quantity,
                        RetailCurrency = prod.RetailCurrency,
                        WholeSaleCurrency = prod.WholeSaleCurrency,
                        ShippingCost = prod.ShippingCost,
                        CreatedOn = prod.CreatedOn,
                        UpdatedOn = prod.UpdatedOn,
                        IsActive = prod.IsActive,

                        ProductImage = productImageDto
                                        .Where(imageDetails => imageDetails.ProductID == prod.ProductID) // Filter images by ProductID
                                        .Select(imageDetails => new
                                        {
                                            ProductImageURL = imageDetails.ProductImageURL
                                        }).ToList()
                    }).ToList(),
                    TotalPages = totalPages,
                    SearchText = searchText ?? "",
                    Category = category,
                    MinWholesalePrice = minWholesalePrice,
                    MaxWholesalePrice = maxWholesalePrice,
                    MinRetailPrice = minRetailPrice,
                    MaxRetailPrice = maxRetailPrice,
                    Quantity = quantity,
                    SortDescending = sortDescending ?? false,
                    PageNumber = pageNumberValue,
                    PageSize = pageSizeValue
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while retrieving the products.", ErrorMessage = ex.Message });
            }
        }


        //[HttpGet]
        //[Route("{id}")]
        //public async Task<IActionResult> GetProductById(Guid id)
        //{
        //    try
        //    {
        //        var productById = await productRepository.GetProductByIdAsync(id);
        //        var productImages = await productImageRepository.GetProductImageByIdAsync(id);

        //        if (productById == null)
        //        {
        //            return NotFound(new
        //            {
        //                Message = "Product not found.",
        //                ProductCode = string.Empty,
        //                ErrorMessage = "Invalid product ID."
        //            });
        //        }

        //        // Use AutoMapper to map the Product entity to ProductDTO.
        //        //var productDto = _mapper.Map<List<ProductDTO>>(productById);
        //        var productDto = _mapper.Map<ProductDTO>(productById); 
        //        //var productImageDto = _mapper.Map<List<ProductImageDTO>>(productImages);


        //        if (productImages != null)
        //        {
        //            productDto = productImages.ProductImageURL;
        //        }

        //        return Ok(new
        //        {
        //            Message = "Product retrieved successfully.",
        //            Product = productDto,                   
        //            ErrorMessage = string.Empty
        //        });


        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new
        //        {
        //            Message = "An error occurred while retrieving the product.",
        //            ProductCode = string.Empty,
        //            ErrorMessage = ex.Message
        //        });
        //    }
        //}

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetProductById(Guid id)
        {
            try
            {
                var products = await productRepository.GetProductByIdAsync(id);

                if (products == null || !products.Any())
                {
                    return Ok(new
                    {
                        Message = "Failed",
                        ErrorMessage = "No products found for the provided Product ID."
                    });
                }

                // Get related product image details for each product
                var productImage = await productImageRepository.FindByCondition(prd => products.Select(p => p.ProductID).Contains(prd.ProductID)).ToListAsync();

                // Map entities to DTOs
                var productsDto = _mapper.Map<List<ProductDTO>>(products);
                var productImageDto = _mapper.Map<List<ProductImageDTO>>(productImage);

                return Ok(new
                {

                    Message = "Product fetched successfully.",
                    ErrorMessage = string.Empty,
                    Product = productsDto.Select(prod => new
                    {
                        ProductID = prod.ProductID,
                        ProductCode = prod.ProductCode,
                        ManufacturerID = prod.ManufacturerID,
                        ProductName = prod.ProductName,
                        Description = prod.Description,
                        Category = prod.Category,
                        WholesalePrice = prod.WholesalePrice,
                        RetailPrice = prod.RetailPrice,
                        Quantity = prod.Quantity,
                        RetailCurrency = prod.RetailCurrency,
                        WholeSaleCurrency = prod.WholeSaleCurrency,
                        ShippingCost = prod.ShippingCost,
                        CreatedOn = prod.CreatedOn,
                        UpdatedOn = prod.UpdatedOn,
                        IsActive = prod.IsActive,

                        ProductImage = productImageDto.Select(imageDetails => new
                        {
                            ProductImageURL = imageDetails.ProductImageURL

                        }).ToList()
                    }).ToList()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "An error occurred while retrieving the product for - ",
                    ManufacturerId = id,
                    ErrorMessage = ex.Message
                });
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

                // Use AutoMapper to update the existing product with values from the DTO.
                _mapper.Map(updateProductRequestDto, existingProduct[0]);

                // Optionally update properties that aren’t handled by AutoMapper.
                existingProduct[0].UpdatedOn = DateTime.UtcNow;

                // Update the product in the repository
                var updatedProduct = await productRepository.UpdateProductAsync(id, existingProduct[0]);

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

        [HttpPatch("{id}/UpdateProductQuantity")]
        public async Task<IActionResult> UpdateProductQuantity(Guid id, [FromBody] ProductQuantityUpdateDTO updateDto)
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
                        ErrorMessage = "Invalid product ID."
                    });
                }

                existingProduct[0].Quantity = updateDto.Quantity;
                existingProduct[0].UpdatedOn = DateTime.UtcNow;

                // Save changes
                var updatedProduct = await productRepository.UpdateProductQuantityAsync(id, existingProduct[0]);

                if (updatedProduct == null)
                {
                    return StatusCode(500, new
                    {
                        Message = "Failed to update product quantity.",
                        ErrorMessage = "Internal server error."
                    });
                }

                return Ok(new
                {
                    Message = "Product quantity updated successfully.",
                    ProductID = updatedProduct.ProductID,
                    UpdatedQuantity = updatedProduct.Quantity,
                    UpdatedOn = updatedProduct.UpdatedOn
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "An error occurred while updating the product quantity.",
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

                productById[0].IsActive = false;

                await productRepository.UpdateProductAsync(id, productById[0]);
                return Ok(new
                {
                    Message = "Product deleted successfully.",
                    ProductCode = productById[0].ProductCode,
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


        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromForm] CreateProductDTO addProductRequestDto, IFormFile productImage)
        {
            string imageFileName = string.Empty;
            try
            {
                // Step 1: Handle Image Upload                
                if (productImage != null)
                {
                    // Upload image and get the file name
                    imageFileName = await UploadImage(productImage);
                }

                // Step 2: Map the incoming CreateProductDTO to a Product entity
                var productModel = _mapper.Map<Product>(addProductRequestDto);
                productModel.CreatedOn = DateTime.UtcNow;
                productModel.UpdatedOn = DateTime.UtcNow;
                productModel.IsActive = true;

                // Step 3: Use Repository to create Product
                productModel = await productRepository.CreateProductAsync(productModel);

                // Step 4: Insert image data into ProductImage table if an image is uploaded
                if (!string.IsNullOrEmpty(imageFileName))
                {
                    var productImageModel = new ProductImage
                    {
                        ImageID = Guid.NewGuid(),
                        ProductID = productModel.ProductID,
                        ProductImageURL = "/uploads/images/" + imageFileName, // Or you can store the full path depending on your need
                        FileName = imageFileName,
                        FileExtension = Path.GetExtension(imageFileName)
                    };

                    await productRepository.InsertProductImageAsync(productImageModel);  // Insert the image data into the DB
                }

                // Step 5: Return response with product creation details
                var productDto = _mapper.Map<ProductDTO>(productModel);
                var response = new APIResponse
                {
                    Message = "Product created successfully.",
                    FileName = imageFileName,
                    ErrorMessage = string.Empty
                };

                return CreatedAtAction(nameof(GetProductById), new { id = productModel.ProductID }, response);
            }
            catch (Exception ex)
            {
                var response = new APIResponse
                {
                    Message = "Product creation failed.",
                    FileName = string.Empty,
                    ErrorMessage = ex.Message
                };

                // Step 6: Clean up the uploaded image if product creation fails
                if (!string.IsNullOrEmpty(imageFileName))
                {
                    DeleteImage(imageFileName);  // Call function to delete the image file
                }

                return StatusCode(500, response);
            }
        }


        private async Task<string> UploadImage(IFormFile productImage)
        {
            try
            {
                // Ensure the folder exists
                string uploadsFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "images");

                // Create the folder if it doesn't exist
                if (!Directory.Exists(uploadsFolderPath))
                {
                    Directory.CreateDirectory(uploadsFolderPath);
                }

                // Get the file name and path
                string fileExtension = Path.GetExtension(productImage.FileName);
                string fileName = Guid.NewGuid().ToString() + fileExtension;
                string filePath = Path.Combine(uploadsFolderPath, fileName);

                // Save the image to the server's file system
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await productImage.CopyToAsync(fileStream);
                }

                return fileName;  // Return the saved file's name
            }
            catch (Exception ex)
            {
                // Handle any errors during the file upload
                throw new Exception("Error uploading image: " + ex.Message);
            }
        }

        private void DeleteImage(string imageFileName)
        {
            try
            {
                var filePath = Path.Combine("wwwroot/uploads/images", imageFileName); // Adjust the path if needed
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {
                // Log the error if needed
                Console.WriteLine($"Error deleting image: {ex.Message}");
            }
        }


    }
}

