using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using StorApp.Dtos;
using StorApp.Model;
using StorApp.Services;

namespace StorApp.Controllers
{
    [Route("api/Products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ILogger<ProductsController> logger;
        private readonly IProductsService Service;

        public ProductsController(ILogger<ProductsController> logger, IProductsService Service)
        {
            this.logger = logger;
            this.Service = Service;
        }

        [HttpGet("{productId}", Name = "GetProduct")]
        public ActionResult GetProduct(int productId)
        {
            try
            {
                //throw new Exception();
                var products = Service.GetById(productId);
                if (products == null)
                {
                    logger.LogInformation($"The Product With Id {productId} Couldnt be found!");
                    return NotFound();
                }
                return Ok(products);
            }
            catch (Exception ex)
            {
                logger.LogCritical($"Exception While handling a request to Product {productId} ", ex);
                return StatusCode(500, "try again or call the app administrator!");
            }
        }

        [HttpGet]
        public ActionResult GetProducts()
        {

            var products = Service.GetAll();
            return Ok(products);

        }


        [HttpPost]
        public ActionResult Create(CreateProductDto dto)
        {
            var product = new Product()
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Amount = dto.Amount,

            };


            Service.Add(product);

            return CreatedAtRoute("GetProduct", new
            {
                productId = product.ProductId
            }, product);

        }


        [HttpPut("{productId}")]
        public ActionResult UpdateProduct(UpdateProductDto dto, int productId)
        {

            var product = Service.GetById(productId);
            if (product == null)
                return NotFound($"No genre was found with ID: {productId}");

            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.Amount = dto.Amount;

            Service.Update(product);

            return NoContent();
        }


        [HttpPatch("{productId}")]
        public ActionResult PartiallyUpdateProduct(JsonPatchDocument<UpdateProductDto> dto, int productId)
        {

            var existingproduct = Service.GetById(productId);
            if (existingproduct == null)
                return NotFound($"No genre was found with ID: {productId}");

            Service.PartiallyUpdate(dto, existingproduct);
            return NoContent();
        }



        [HttpDelete("{productId}")]
        public ActionResult DeleteProduct(int productId)
        {

            var product = Service.GetById(productId);
            if (product == null)
                return NotFound($"No genre was found with ID: {productId}");
            Service.Delete(product);
            return NoContent();
        }


    }
}