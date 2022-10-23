using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using StorApp.Dtos;
using StorApp.Model;

namespace StorApp.Services
{
    public class ProductsRepository : IProductsRepository
    {
        private readonly ILogger<ProductsRepository> logger;
        private readonly StorDbContext context;


        public ProductsRepository(ILogger<ProductsRepository> logger, StorDbContext context)
        {
            this.logger = logger;
            this.context = context;

        }
        public async Task<IList<Brand>?> GetBrandsAsync()
        {
            return await context.Products.SelectMany(p => p.Brands).Distinct().ToListAsync();
        }

        public async Task<(IList<Product>?, PaginationMetaData)> GetProductsAsync(int pageNumber, int pageSize, string? name, int? maxPrice, int minPrice = 0)
        {
            var totalProducts = await context.Products.CountAsync();

            var paginationMetaData = new PaginationMetaData(totalProducts, pageSize, pageNumber);

            var query = context.Products as IQueryable<Product>;

            if (!string.IsNullOrEmpty(name))
            {
                name = name.ToLower();
                query = query.Where(x => x.Name.ToLower().Contains(name));
            }

            if (maxPrice != null)
                query = query.Where(p => p.Price * 1.12 <= maxPrice);

            if (minPrice > 0)
                query = query.Where(p => p.Price * 1.12 >= minPrice);

            var filterProducts = await query
                .OrderBy(p => p.Name)
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .ToListAsync();

            return (filterProducts, paginationMetaData);
        }
      

        public async Task<Product?> GetProductAsync(int productId, bool includeBrands = false)
        {

            if (includeBrands)
                return await context.Products
                   .Where(b => b.ProductId == productId)
                   .Include(p => p.Brands)
                   .FirstOrDefaultAsync();
            else
                return await context.Products
                      .Where(b => b.ProductId == productId)
                      .FirstOrDefaultAsync();
        }
        public async Task<Product> AddProductAsync(Product product)
        {
            await context.Products.AddAsync(product);
            await context.SaveChangesAsync();
            return product;
        }

        public async Task UpdateProductAsync(Product product)
        {
            context.Products.Update(product);
            await context.SaveChangesAsync();
        }

        public async Task PartiallyUpdateProductAsync(JsonPatchDocument<ProductDto> dto, Product product)
        {
            var productToPatch = new ProductDto()
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

            context.Products.Update(product);
            await context.SaveChangesAsync();
        }
        public async Task DeleteAsync(Product product)
        {
            //context.Products.Remove(product);
            product.IsDeleted = true;
            await UpdateProductAsync(product);
            await context.SaveChangesAsync();
        }
    }
}