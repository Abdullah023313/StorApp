using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using StorApp.Dtos;
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

    public class ProductsController : ControllerBase
    {
        private readonly ILogger<ProductsController> logger;
        private readonly IProductsRepository Service;
        private readonly IMapper mapper;
        private readonly IMailServices mail;
        private readonly int maxPageSize = 50;

        public ProductsController(ILogger<ProductsController> logger, IProductsRepository Service , IMapper mapper , IMailServices mail)
        {
            this.logger = logger;
            this.Service = Service;
            this.mapper = mapper;
            this.mail = mail;
        }
        [HttpGet(template: "AllBrands")]
        public async Task<ActionResult> GetBrands()
        {
            var brands = await Service.GetBrandsAsync();
            if (brands == null)
            {
                logger.LogInformation($"NULL!");
                return NotFound($"NULL");
            }
            return Ok(brands);
        }


        [HttpGet("{productId}", Name = "GetProduct")]
        public async Task<ActionResult> GetProduct(int productId)
        {
            var products = await Service.GetProductAsync(productId,true);
            if (products == null)
            {
                logger.LogInformation($"The product with ID {productId} could not be found!");
                return NotFound();
            }
            return Ok(mapper.Map<ProductWithBrands>(products));
        }

        [HttpGet]
        public async Task<ActionResult> GetProducts(string? name, int? maxPrice, int minPrice = 0,int PageNumber=1, int pageSize = 10)
        {
            pageSize = pageSize > maxPageSize ? maxPageSize : pageSize;

            var (products, paginationData) = await Service.GetProductsAsync(PageNumber, pageSize, name:name, maxPrice:maxPrice,minPrice);

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
          
            product.Name=dto.Name;
            product.Description=dto.Description;
            product.Price=dto.Price;
            product.Amount=dto.Amount;
            
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
            mail.Send(productId);
            return NoContent();
        }

    }
}