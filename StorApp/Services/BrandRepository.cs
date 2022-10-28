
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using StorApp.Dtos;
using StorApp.Model;

namespace StorApp.Services
{
    public class BrandRepository : IBrandRepository
    {
        private readonly ILogger<BrandRepository> logger;
        private readonly StorDbContext context;


        public BrandRepository(ILogger<BrandRepository> logger, StorDbContext context)
        {
            this.logger = logger;
            this.context = context;

        }


        public async Task<IList<Brand>?> GetBrandsForProductAsync(int productId)
        {
            return await context.Brands.Where(b => b.ProductId == productId).ToListAsync();
        }

        public async Task<Brand?> GetBrandForProductAsync(int productId, int brandId)
        {
            return await context.Brands.Where(b => b.ProductId == productId && b.BrandId == brandId).FirstOrDefaultAsync();
        }

        public async Task<Brand> AddBrandAsync(Brand brand)
        {

            await context.Brands.AddAsync(brand);
            await context.SaveChangesAsync();
            return brand;
        }


        public async Task UpdateBrandAsync(Brand brand)
        {
            context.Brands.Update(brand);
            await context.SaveChangesAsync();
        }

        public async Task PartiallyUpdateBrandAsync(JsonPatchDocument<BrandDto> dto, Brand brand)
        {
            var brandToPatch = new BrandDto()
            {
                Name = brand.Name,
                Notes = brand.Notes
            };

            dto.ApplyTo(brandToPatch);

            brand.Name = brandToPatch.Name;
            brand.Notes = brandToPatch.Notes;

            context.Brands.Update(brand);
            await context.SaveChangesAsync();
        }

        public async Task DeleteBrandAsync(Brand brand)
        {
            context.Brands.Remove(brand);
            await context.SaveChangesAsync();
        }
    }
}
