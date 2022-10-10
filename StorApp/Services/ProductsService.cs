using Microsoft.AspNetCore.JsonPatch;
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
        public Product Add(Product product)
        {

            context.Products.Add(product);
            context.SaveChanges();
            return product;
        }

        public void Delete(Product product)
        {

            context.Products.Remove(product);
            context.SaveChanges();
        }

        public IList<Product> GetAll()
        {

            var products = context.Products.OrderBy(p => p.Name).ToList();
            return products;
        }

        public Product GetById(int id)
        {

            var product = context.Products.SingleOrDefault(p => p.ProductId == id);
            if (product == null)
            {
                logger.LogInformation(message: $"The Product With Id {id} Couldnt be found!");
            }

            return product;
        }




        public void PartiallyUpdate(JsonPatchDocument<UpdateProductDto> dto, Product product)
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
            context.SaveChanges();
        }


        public void Update(Product product)
        {

            context.Products.Update(product);
            context.SaveChanges();
        }
    }
}