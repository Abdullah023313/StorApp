using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using StorApp.Dtos;
using StorApp.Model;
using StorApp.Services.StorApi.Services;
using StorApp.Services;
using System.Drawing.Drawing2D;

namespace StorApp.Controllers
{
    [Route("api/products/{productId}/brands")]
    [ApiController]
    public class BrandsController : ControllerBase
    {
        private readonly ILogger<BrandsController> logger;
        private readonly IBrandRepository Service;
        private readonly IMapper mapper;

        public BrandsController(ILogger<BrandsController> logger, IBrandRepository Service, IMapper mapper)
        {
            this.logger = logger;
            this.Service = Service;
            this.mapper = mapper;

        }

        [HttpGet]
        public async Task<ActionResult> GetBrands(int productId)
        {
            IList<Brand>? brands;
            if (productId > 0)
            {
                 brands = await Service.GetBrandsForProductAsync(productId);
            }
            else
            {
                 brands = await Service.GetBrandsAsync();
            }             
            if (brands == null)
            {
                logger.LogInformation($"There are no product {productId} brands!");
                return NotFound($"The brand with ID {productId} could not be found!");
            }
            return Ok(brands);

        }


        [HttpGet("{brandId}", Name = "GetBrand")]
        public async Task<ActionResult> GetBrand(int productId, int brandId)
        {
            var brand = await Service.GetBrandForProductAsync(productId, brandId);
            return Ok(brand);
        }


        [HttpPost]
        public async Task<ActionResult> Create(CreateBrandsDto dto, int ProductId)
        {
            var brand = new Brand()
            {
                Name = dto.Name,
                Notes = dto.Notes,
                ProductId = ProductId
            };
            await Service.AddBrandAsync(brand);

            return CreatedAtRoute("GetBrand", new
            {
                productId = brand.ProductId,
                brandId = brand.BrandId
            }, brand);

        }


        [HttpPut("{brandId}")]
        public async Task<ActionResult> UpdateBrand(UpdateBrandDto dto, int productId, int brandId)
        {

            var brand = await Service.GetBrandForProductAsync(productId, brandId);
            if (brand == null)
            {
                logger.LogInformation($"The brand with ID {brandId} could not be found!");
                return NotFound($"The brand with ID {brandId} could not be found!");
            }

            brand.Name = dto.Name;
            brand.Notes = dto.Notes;


            await Service.UpdateBrandAsync(brand);

            return NoContent();
        }


        [HttpPatch("{brandId}")]
        public async Task<ActionResult> PartiallyUpdateBrand(JsonPatchDocument<UpdateBrandDto> dto, int productId, int brandId)
        {

            var existingbrand = await Service.GetBrandForProductAsync(productId, brandId);
            if (existingbrand == null)
            {
                logger.LogInformation($"The brand with ID {brandId} could not be found!");
                return NotFound($"The brand with ID {brandId} could not be found!");
            }
            await Service.PartiallyUpdateBrandAsync(dto, existingbrand);
            return NoContent();
        }



        [HttpDelete("{brandId}")]
        public async Task<ActionResult> DeleteProduct(int productId, int brandId)
        {

            var product = await Service.GetBrandForProductAsync(productId, brandId);
            if (product == null)
            {
                logger.LogInformation($"The brand with ID {brandId} could not be found!");
                return NotFound($"The brand with ID {brandId} could not be found!");
            }
            await Service.DeleteBrandAsync(product);
            return NoContent();
        }


    }
}