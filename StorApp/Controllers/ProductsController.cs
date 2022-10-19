using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using StorApp.Dtos;
using StorApp.Model;
using StorApp.Services;
using StorApp.Services.StorApi.Services;

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

        public ProductsController(ILogger<ProductsController> logger, IProductsRepository Service , IMapper mapper , IMailServices mail)
        {
            this.logger = logger;
            this.Service = Service;
            this.mapper = mapper;
            this.mail = mail;
        }


        [HttpGet("{productId}", Name = "GetProduct")]
        public async Task<ActionResult> GetProduct(int productId)
        {
            var products = await Service.GetProductAsync(productId);
            if (products == null)
            {
                logger.LogInformation($"The product with ID {productId} could not be found!");
                return NotFound();
            }
            return Ok(mapper.Map<ProductWithoutBrands>(products));
        }

        [HttpGet]
        public async Task<ActionResult> GetProducts()
        {
            var products = await Service.GetProductsAsync();
            return Ok(mapper.Map<List<ProductWithoutBrands>>(products));

        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateProductDto dto)
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
        public async Task<ActionResult> UpdateProduct(UpdateProductDto dto, int productId)
        {

            var product = await Service.GetProductAsync(productId);
            if (product == null)
            {
                logger.LogInformation($"The product with ID {productId} could not be found!");
                return NotFound($"The product with ID {productId} could not be found!");
            }
          
            product=mapper.Map<Product>(dto);
            await Service.UpdateProductAsync(product);

            return NoContent();
        }


        [HttpPatch("{productId}")]
        public async Task<ActionResult> PartiallyUpdateProduct(JsonPatchDocument<UpdateProductDto> dto, int productId)
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