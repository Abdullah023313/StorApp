using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IIS.Core;
using StorApp.Dtos;
using StorApp.Extensions;
using StorApp.Model;
using StorApp.Model.Dtos;
using StorApp.Services;
using StorApp.Services.StorApi.Services;
using System.Drawing.Printing;
using System.Text.Json;

namespace StorApp.Controllers
{
    [Route("api/Products")]
    [ApiController]
    //[Authorize]
    //[Authorize(Policy="SuperAdmin")]
    //[Authorize(Roles = "SuperAdminstrator , Adminstrator ")]
    public class ProductsController : ControllerBase
    {
        private readonly ILogger<ProductsController> logger;
        private readonly IProductsRepository Service;
        private readonly IMapper mapper;
        private readonly int maxPageSize = 50;

        public ProductsController(ILogger<ProductsController> logger, IProductsRepository Service, IMapper mapper)
        {
            this.logger = logger;
            this.Service = Service;
            this.mapper = mapper;
        }

   
        [HttpGet(template: "AllBrands")]
        public async Task<ActionResult> GetBrands()
        {
            var brands = await Service.GetBrandsAsync();
            if (brands == null)
            {
                return NotFound($"NULL");
                logger.LogInformation("Not Found brands ", "NullReferenceException");
            }
            return Ok(brands);
        }


        [HttpGet("{productId}", Name = "GetProduct")]
        public async Task<ActionResult> GetProduct(int productId)
        {
            var products = await Service.GetProductAsync(productId, true);
            if (products == null)
            {
                logger.myLogInformation($"The product with ID {productId} could not be found! ", new NullReferenceException());
                return NotFound();
            }
            return Ok(mapper.Map<ProductWithBrands>(products));
        }

        /// <summary>
        /// Get list of Products
        /// </summary>
        /// <param name="name">Search by product name</param>
        /// <param name="maxPrice">Determine the highest price for the products</param>
        /// <param name="minPrice">The lowest price for products, the default value is zero</param>
        /// <param name="PageNumber">Page number, the default value is zero</param>
        /// <param name="pageSize">Page size, the default value is ten</param>
        /// <returns>List of products, maximum number of products {pageSize} </returns>
        [HttpGet]
        public async Task<ActionResult> GetProducts(string? name, int? maxPrice, int minPrice = 0, int PageNumber = 1, int pageSize = 10)
        {
            pageSize = pageSize > maxPageSize ? maxPageSize : pageSize;

            var (products, paginationData) = await Service.GetProductsAsync(PageNumber, pageSize, name: name, maxPrice: maxPrice, minPrice);

            Response.Headers.Add("X-pagination", paginationData.ToString());
   

            return Ok(mapper.Map<List<ProductWithoutBrands>>(products));

        }

        [HttpPost]
        public async Task<ActionResult> Create(ProductDto dto)
        {
            var product = new Product()
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Amount = dto.Amount,

            };
            await Service.AddProductAsync(product);

            return CreatedAtRoute("GetProduct", new
            {
                productId = product.ProductId
            }, product);
        }


        [HttpPut("{productId}")]
        public async Task<ActionResult> UpdateProduct(ProductDto dto, int productId)
        {

            var product = await Service.GetProductAsync(productId);
            if (product == null)
            {
                logger.LogInformation($"The product with ID {productId} could not be found!");
                return NotFound($"The product with ID {productId} could not be found!");
            }

            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.Amount = dto.Amount;

            await Service.UpdateProductAsync(product);

            return NoContent();
        }


        [HttpPatch("{productId}")]
        public async Task<ActionResult> PartiallyUpdateProduct(JsonPatchDocument<ProductDto> dto, int productId)
        {

            var existingproduct = await Service.GetProductAsync(productId);
            if (existingproduct == null)
            {
                logger.LogInformation($"The product with ID {productId} could not be found!");
                return NotFound($"The product with ID {productId} could not be found!");
            }
            await Service.PartiallyUpdateProductAsync(dto, existingproduct);
            return NoContent();
        }

        [HttpDelete("{productId}")]
        public async Task<ActionResult> DeleteProduct(int productId)
        {

            var product = await Service.GetProductAsync(productId);
            if (product == null)
            {
                logger.LogInformation($"The product with ID {productId} could not be found!");
                return NotFound($"The product with ID {productId} could not be found!");
            }
            await Service.DeleteAsync(product);
            return NoContent();
        }

    }
}