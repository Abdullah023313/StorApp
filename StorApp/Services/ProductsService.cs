using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using StorApp.Dtos;
using StorApp.Model;

namespace StorApp.Services
{
    public class ProductsService : IProductsService
    {
        private readonly ILogger<ProductsService> logger;
        private readonly StorDbContext context;

        public ProductsService(ILogger<ProductsService> logger, StorDbContext context)
        {
            this.logger = logger;
            this.context = context;
        }

        public async Task<Product> AddAsync(Product product)
        {

            await context.Products.AddAsync(product);
            await context.SaveChangesAsync();
            return product;
        }

        public async Task DeleteAsync(Product product)
        { 
            context.Products.Remove(product);
            await context.SaveChangesAsync();
        }

        public async Task<IList<Product>?> GetAllAsync()
        {
            return await context.Products.OrderBy(p => p.Name).ToListAsync();
        }


        public async Task<Product?> GetByIdAsync(int id)
        {
            var product = await context.Products.SingleOrDefaultAsync(p => p.ProductId == id);
            if (product == null)
            {
                logger.LogInformation(message: $"The Product With Id {id} Couldnt be found!");
            }
            return product;
        }

        public async Task PartiallyUpdateAsync(JsonPatchDocument<UpdateProductDto> dto, Product product)
        {

            var productToPatch = new UpdateProductDto()
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Amount = product.Amount
            };

            dto.ApplyTo(productToPatch);

            product.Name = productToPatch.Name;
            product.Description = productToPatch.Description;
            product.Price = productToPatch.Price;
            product.Amount = productToPatch.Amount;


            context.Products.Update(product);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product product)
        {
            context.Products.Update(product);
            await context.SaveChangesAsync();
        }
    }
}