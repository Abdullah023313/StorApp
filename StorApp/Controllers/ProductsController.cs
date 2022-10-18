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
        private readonly IProductsService Service;
        private readonly IMapper mapper;
        private readonly IMailServices mail;

        public ProductsController(ILogger<ProductsController> logger, IProductsService Service , IMapper mapper , IMailServices mail)
        {
            this.logger = logger;
            this.Service = Service;
            this.mapper = mapper;
            this.mail = mail;
        }
        

        [HttpGet("{productId}", Name = "GetProduct")]
        public async Task <ActionResult> GetProduct(int productId)
        {
            try
            {
                //throw new Exception();
                var products = await Service.GetByIdAsync(productId);
                if (products == null)
                {
                    logger.LogInformation($"The Product With Id {productId} Couldnt be found!");
                    return NotFound();
                }
                return Ok(products);
            }
            catch (Exception ex)
            {
                logger.LogError($"Exception While handling a request to Product {productId} ", ex);
                return StatusCode(500, "try again or call the app administrator!");
            }
        }

        [HttpGet]
        public async Task<ActionResult> GetProducts()
        {

            var products = await Service.GetAllAsync();
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
           await Service.AddAsync(product);

            return CreatedAtRoute("GetProduct", new
            {
                productId = product.ProductId
            }, product);

        }


        [HttpPut("{productId}")]
        public async Task<ActionResult> UpdateProduct(UpdateProductDto dto, int productId)
        {

            var product = await Service.GetByIdAsync(productId);
            if (product == null)
                return NotFound($"No genre was found with ID: {productId}");

            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.Amount = dto.Amount;

            await Service.UpdateAsync(product);

            return NoContent();
        }


        [HttpPatch("{productId}")]
        public async Task<ActionResult> PartiallyUpdateProduct(JsonPatchDocument<UpdateProductDto> dto, int productId)
        {

            var existingproduct = await Service.GetByIdAsync(productId);
            if (existingproduct == null)
                return NotFound($"No genre was found with ID: {productId}");

            await Service.PartiallyUpdateAsync(dto, existingproduct);
            return NoContent();
        }



        [HttpDelete("{productId}")]
        public async Task<ActionResult> DeleteProduct(int productId)
        {

            var product =await Service.GetByIdAsync(productId);
            if (product == null)
            {
                var products = await Service.GetAllAsync();
                product = products?.FirstOrDefault(); ;
                if (product == null)
                {
                    return NotFound($"No genre was found with ID: {productId}");
                }
            }
            await Service.DeleteAsync(product);
            mail.Send();
            return NoContent();
        }


    }
}