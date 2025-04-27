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
using ProductManagement.Logger.interfaces;


namespace ProductManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]  
    [Authorize]

    public class ProductManagementController : ControllerBase
    {

        private readonly AppDbContext dbContext;
        private readonly IProductRepository productRepository;
        private readonly IProductImageRepository productImageRepository;
        private readonly IMapper _mapper;
        private readonly IAppLogger<ProductManagementController> _logger;
        public ProductManagementController(AppDbContext appDbContext, IProductRepository prodRepo, IProductImageRepository productImageRepo, IMapper mapper, IAppLogger<ProductManagementController> logger)
        {
            this.dbContext = appDbContext;
            this.productRepository = prodRepo;
            this.productImageRepository = productImageRepo;
            _mapper = mapper;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {        
            _logger.LogInformation("Entering GetAllProducts API...");
            try
            {
                _logger.LogInformation("Retrieving all active products from Database...");
                var productsModel = await productRepository.GetAllProductsAsync();

                if (productsModel == null || !productsModel.Any())
                {
                    _logger.LogWarning("No products found");
                    return NotFound(new
                    {
                        Message = "No products found.",
                        ErrorMessage = "No data available."
                    });
                }

                // Get related Product Image details for each product
                var productImage = await productImageRepository.FindByCondition(prd => productsModel.Select(p => p.ProductID).Contains(prd.ProductID)).ToListAsync();

                // Use AutoMapper to map the list of Product entities to ProductDTOs.
                _logger.LogInformation("Mapping Product DTO to Product Model..");
                var productDTOs = _mapper.Map<List<ProductDTO>>(productsModel);
                _logger.LogInformation("Mapping Product Image DTO to Product Image Model...");
                var productImageDto = _mapper.Map<List<ProductImageDTO>>(productImage);

                _logger.LogInformation("Retrieved {ProductCount} products", productDTOs.Count);

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
                _logger.LogError(ex, "An error occurred while retrieving the products",ex.Message);
                return StatusCode(500, new
                {
                    Message = "An error occurred while retrieving the products.",
                    ErrorMessage = ex.Message
                });
            }
        }

        [AllowAnonymous]
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
            _logger.LogInformation("Entering GetFilteredProducts API...");
            _logger.LogInformation("Filter parameters: searchText={SearchText}, category={Category}, minWholesalePrice={MinWholesalePrice}, maxWholesalePrice={MaxWholesalePrice}, minRetailPrice={MinRetailPrice}, maxRetailPrice={MaxRetailPrice}, quantity={Quantity}, sortDescending={SortDescending}, pageNumber={PageNumber}, pageSize={PageSize}",
                searchText, category, minWholesalePrice, maxWholesalePrice, minRetailPrice, maxRetailPrice, quantity, sortDescending, pageNumber, pageSize);

            try
            {
                int pageNumberValue = pageNumber ?? 1;
                int pageSizeValue = pageSize ?? 10;

                _logger.LogInformation("Filtering all active products from Database...");
                var products = await productRepository.GetFilteredProductsAsync(
                    searchText, category, minWholesalePrice, maxWholesalePrice, minRetailPrice,
                    maxRetailPrice, quantity, sortDescending, pageNumberValue, pageSizeValue);

                if (!products.Any())
                {
                    _logger.LogWarning("No results found");
                    return NotFound(new { Message = "No results found. Please adjust your filters.", ErrorMessage = "No data available." });
                }

                // Get related Product Image details for each product
                var productImage = await productImageRepository.FindByCondition(prd => products.Select(p => p.ProductID).Contains(prd.ProductID)).ToListAsync();

                int totalProducts = await productRepository.GetTotalProductCountAsync(searchText, category, minWholesalePrice, maxWholesalePrice, minRetailPrice, maxRetailPrice, quantity);
                int totalPages = (int)Math.Ceiling((double)totalProducts / pageSizeValue);

                // Use AutoMapper to map the list of Product entities to ProductDTOs.
                _logger.LogInformation("Mapping Product DTO to Product Model...");
                var productDTOs = _mapper.Map<List<ProductDTO>>(products);
                _logger.LogInformation("Mapping Product Image DTO to Product Image Model...");
                var productImageDto = _mapper.Map<List<ProductImageDTO>>(productImage);

                _logger.LogInformation("Retrieved {ProductCount} products", productDTOs.Count);

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
                _logger.LogError(ex, "An error occurred while retrieving the products",ex.Message.ToString());
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
            _logger.LogInformation("Entering GetProductById API...");
            

            try
            {
                _logger.LogInformation("Querying product with ID: {ProductId}", id);
                var products = await productRepository.GetProductByIdAsync(id);

                if (products == null || !products.Any())
                {
                    _logger.LogWarning("No products found for the provided Product ID: {ProductId}", id);
                    return Ok(new
                    {
                        Message = "Failed",
                        ErrorMessage = "No products found for the provided Product ID."
                    });
                }

                // Get related product image details for each product
                var productImage = await productImageRepository.FindByCondition(prd => products.Select(p => p.ProductID).Contains(prd.ProductID)).ToListAsync();

                // Map entities to DTOs
                _logger.LogInformation("Mapping Product DTO to Product Model...");
                var productsDto = _mapper.Map<List<ProductDTO>>(products);
                _logger.LogInformation("Mapping Product Image DTO to Product Image Model...");
                var productImageDto = _mapper.Map<List<ProductImageDTO>>(productImage);

                _logger.LogInformation("Retrieved {ProductCount} products for Product ID: {ProductId}", productsDto.Count, id);

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
                _logger.LogError(ex, "An error occurred while retrieving the product for Product ID: {ProductId}", id);
                return StatusCode(500, new
                {
                    Message = "An error occurred while retrieving the product.",
                    ManufacturerId = id,
                    ErrorMessage = ex.Message
                });
            }
        }


        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] UpdateProductDTO updateProductRequestDto)
        {
            _logger.LogInformation("Entering UpdateProduct API...");
        

            try
            {
                // Check if the product exists
                _logger.LogInformation("Getting product by ID: {ProductId}", id);
                var existingProduct = await productRepository.GetProductByIdAsync(id);
                if (existingProduct == null)
                {
                    _logger.LogWarning("Product not found with ID: {ProductId}", id);
                    return NotFound(new
                    {
                        Message = "Product not found.",
                        ProductCode = string.Empty,
                        ErrorMessage = "Invalid product ID."
                    });
                }

                // Use AutoMapper to update the existing product with values from the DTO.
                _logger.LogInformation("Mapping UpdateProductDTO to Product model...");
                _mapper.Map(updateProductRequestDto, existingProduct[0]);

                // Optionally update properties that aren’t handled by AutoMapper.
                existingProduct[0].UpdatedOn = DateTime.UtcNow;

                _logger.LogInformation("Updating product with ID: {ProductId}", id);
                // Update the product in the repository
                var updatedProduct = await productRepository.UpdateProductAsync(id, existingProduct[0]);

                if (updatedProduct == null)
                {
                    _logger.LogError(null,"Failed to update product with ID: {ProductId}", id);
                    return StatusCode(500, new
                    {
                        Message = "Failed to update product.",
                        ProductCode = string.Empty,
                        ErrorMessage = "Internal server error."
                    });
                }

                _logger.LogInformation("Product updated successfully with ID: {ProductId}", id);
                return Ok(new
                {
                    Message = "Product updated successfully.",
                    ProductCode = updatedProduct.ProductCode,
                    ErrorMessage = string.Empty
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the product with ID: {ProductId}", id);
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
            _logger.LogInformation("Entering UpdateProductQuantity API...");
        
            try
            {
                // Check if the product exists
                _logger.LogInformation("Getting product by ID: {ProductId}", id);
                var existingProduct = await productRepository.GetProductByIdAsync(id);
                if (existingProduct == null)
                {
                    _logger.LogWarning("Product not found with ID: {ProductId}", id);
                    return NotFound(new
                    {
                        Message = "Product not found.",
                        ErrorMessage = "Invalid product ID."
                    });
                }

                existingProduct[0].Quantity = updateDto.Quantity;
                existingProduct[0].UpdatedOn = DateTime.UtcNow;

                // Save changes
                _logger.LogInformation("Updating quantity for product with ID: {ProductId}", id);
                var updatedProduct = await productRepository.UpdateProductQuantityAsync(id, existingProduct[0]);

                if (updatedProduct == null)
                {
                    _logger.LogError(null, "Failed to update product quantity for ID: {ProductId}", id);
                    return StatusCode(500, new
                    {
                        Message = "Failed to update product quantity.",
                        ErrorMessage = "Internal server error."
                    });
                }

                _logger.LogInformation("Product quantity updated successfully for ID: {ProductId}", id);
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
                _logger.LogError(ex, "An error occurred while updating the product quantity for ID: {ProductId}", id);
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
            _logger.LogInformation("Entering DeleteProduct API...");
      

            try
            {
                // Check if the product exists
                _logger.LogInformation("Getting product by ID: {ProductId}", id);
                var productById = await productRepository.GetProductByIdAsync(id);
                if (productById == null)
                {
                    _logger.LogWarning("Product not found with ID: {ProductId}", id);
                    return NotFound(new
                    {
                        Message = "Product not found.",
                        ProductCode = string.Empty,
                        ErrorMessage = "Invalid product ID."
                    });
                }

                productById[0].IsActive = false;
                _logger.LogInformation("Deleting product with ID: {ProductId}", id);
                await productRepository.UpdateProductAsync(id, productById[0]);

                _logger.LogInformation("Product deleted successfully with ID: {ProductId}", id);
                return Ok(new
                {
                    Message = "Product deleted successfully.",
                    ProductCode = productById[0].ProductCode,
                    ErrorMessage = string.Empty
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the product with ID: {ProductId}", id);
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
            _logger.LogInformation("Entering CreateProduct API...");

            string imageFileName = string.Empty;
            try
            {
                // Step 1: Handle Image Upload
                if (productImage != null)
                {
                    _logger.LogInformation("Uploading product image...");
                    // Upload image and get the file name
                    imageFileName = await UploadImage(productImage);
                }

                // Step 2: Map the incoming CreateProductDTO to a Product entity
                _logger.LogInformation("Mapping CreateProductDTO to Product model...");
                var productModel = _mapper.Map<Product>(addProductRequestDto);
                productModel.CreatedOn = DateTime.UtcNow;
                productModel.UpdatedOn = DateTime.UtcNow;
                productModel.IsActive = true;

                // Step 3: Use Repository to create Product
                _logger.LogInformation("Creating product in the repository...");
                productModel = await productRepository.CreateProductAsync(productModel);

                // Step 4: Insert image data into ProductImage table if an image is uploaded
                if (!string.IsNullOrEmpty(imageFileName))
                {
                    _logger.LogInformation("Inserting product image data into the repository...");
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

                _logger.LogInformation("Product created successfully with ID: {ProductId}", productModel.ProductID);
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
                    _logger.LogInformation("Cleaning up uploaded image due to failure...");
                    DeleteImage(imageFileName);  // Call function to delete the image file
                }

                _logger.LogError(ex, "An error occurred while creating the product");
                return StatusCode(500, response);
            }
        }


        private async Task<string> UploadImage(IFormFile productImage)
        {
            try
            {
                // Ensure the folder exists
                //string uploadsFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "images");

                string uploadsFolderPath = Environment.GetEnvironmentVariable("UPLOAD_PATH") ??
                                           Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "images");


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

