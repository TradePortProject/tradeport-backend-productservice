using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductManagement.Data;
using ProductManagement.Models;
using ProductManagement.Models.DTO;
using ProductManagement.Repositories;


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

        //Get all products
        [HttpGet]
        public async Task<IActionResult> GetAllProdcuts()
        {
            //Get data from the database
            var productsModel = await productRepository.GetAllProdcutsAsync();

            //Map the above date to the DTO object
            var productDTO = new List<ProductDTO>();

            foreach (var product in productsModel)
            {
                productDTO.Add(new ProductDTO()
                {
                    ProductID = product.ProductID,
                    ProductCode = product.ProductCode,
                    ManufacturerID = product.ManufacturerID,
                    ProductName = product.ProductName,
                    Description = product.Description,
                    Category = product.Category,
                    WholesalePrice = product.WholesalePrice,
                    RetailPrice = product.RetailPrice,
                    Quantity = product.Quantity,
                    RetailCurrency = product.RetailCurrency,
                    WholeSaleCurrency = product.WholeSaleCurrency,
                    ShippingCost = product.ShippingCost,
                    CreatedOn = product.CreatedOn,
                    UpdatedOn = product.UpdatedOn,
                    IsActive = product.IsActive
                }
                    );

            }
;
            //Return DTO
            return Ok(productDTO);
        }

        //Get single product
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetProductById(Guid id)
        {
            var prodcutById = await productRepository.GetProductByIdAsync(id);

            if (GetProductById == null)
            {
                return NotFound();
            }

            //Map DTO to the data from DB
            var productDto = new ProductDTO()
            {
                ProductID = prodcutById.ProductID,
                ProductCode = prodcutById.ProductCode,
                ManufacturerID = prodcutById.ManufacturerID,
                ProductName = prodcutById.ProductName,
                Description = prodcutById.Description,
                Category = prodcutById.Category,
                WholesalePrice = prodcutById.WholesalePrice,
                RetailPrice = prodcutById.RetailPrice,
                Quantity = prodcutById.Quantity,
                RetailCurrency = prodcutById.RetailCurrency,
                WholeSaleCurrency = prodcutById.WholeSaleCurrency,
                ShippingCost = prodcutById.ShippingCost,
                CreatedOn = prodcutById.CreatedOn,
                UpdatedOn = prodcutById.UpdatedOn,
                IsActive = prodcutById.IsActive
            };

            return Ok(productDto);

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
                Category = addProductRequestDto.Category,
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

            //Use Model to crate Product
            productModel = await productRepository.CreateProductAsync(productModel);          
            

            //Map the product Model back to DTO
            var productDto = new ProductDTO()
            {
                ProductID = productModel.ProductID,
                ProductCode = productModel.ProductCode,
                ManufacturerID = productModel.ManufacturerID,
                ProductName = productModel.ProductName,
                Description = productModel.Description,
                Category = productModel.Category,
                WholesalePrice = productModel.WholesalePrice,
                RetailPrice = productModel.RetailPrice,
                Quantity = productModel.Quantity,
                RetailCurrency = productModel.RetailCurrency,
                WholeSaleCurrency = productModel.WholeSaleCurrency,
                ShippingCost = productModel.ShippingCost,
                CreatedOn = productModel.CreatedOn,
                UpdatedOn = productModel.UpdatedOn,
                IsActive = productModel.IsActive
            };


            return CreatedAtAction(nameof(GetProductById), new { id = productModel.ProductID }, productModel);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] UpdateProductDTO updateProductRequestDto)
        {

            //Map incoming DTo to Product 

            var productModel = new Product()
            {
                ProductCode = updateProductRequestDto.ProductCode,
                //ManufacturerID = updateProductRequestDto.ManufacturerID,
                ProductName = updateProductRequestDto.ProductName,
                Description = updateProductRequestDto.Description,
                Category = updateProductRequestDto.Category,
                WholesalePrice = updateProductRequestDto.WholesalePrice,
                RetailPrice = updateProductRequestDto.RetailPrice,
                Quantity = updateProductRequestDto.Quantity,
                RetailCurrency = updateProductRequestDto.RetailCurrency,
                WholeSaleCurrency = updateProductRequestDto.WholeSaleCurrency,
                ShippingCost = updateProductRequestDto.ShippingCost,
                CreatedOn = updateProductRequestDto.CreatedOn,
                UpdatedOn = updateProductRequestDto.UpdatedOn,
                IsActive = updateProductRequestDto.IsActive
            };


            //Check if the record of the Id exists
            productModel = await productRepository.UpdateProductAsync(id, productModel);

          if(productModel == null)
            {
                return NotFound();
            }


            //Map the product Model back to DTO
            var productDto = new ProductDTO()
            {
                ProductCode = productModel.ProductCode,
               // ManufacturerID = productModel.ManufacturerID,
                ProductName = productModel.ProductName,
                Description = productModel.Description,
                Category = productModel.Category,
                WholesalePrice = productModel.WholesalePrice,
                RetailPrice = productModel.RetailPrice,
                Quantity = productModel.Quantity,
                RetailCurrency = productModel.RetailCurrency,
                WholeSaleCurrency = productModel.WholeSaleCurrency,
                ShippingCost = productModel.ShippingCost,
                CreatedOn = productModel.CreatedOn,
                UpdatedOn = productModel.UpdatedOn,
                IsActive = productModel.IsActive

            };


            return Ok(productDto);
        }


        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {

            //Check if the record of the Id exists
            var prodcutById = await productRepository.DeleteAysnc(id);

            if (prodcutById == null)
            {
                return NotFound();
            }

            //Use Model to update Product
            dbContext.Products.Remove(prodcutById);
            await dbContext.SaveChangesAsync();            

            return Ok("Record deleted successfully");
        }

    }
}
