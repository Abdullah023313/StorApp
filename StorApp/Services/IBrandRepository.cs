using Microsoft.AspNetCore.JsonPatch;
using StorApp.Dtos;
using StorApp.Model;


namespace StorApp.Services
{
    public interface IBrandRepository
    {
        Task<IList<Brand>?> GetBrandsAsync();
        Task<IList<Brand>?> GetBrandsForProductAsync(int productId);
        Task<Brand?> GetBrandForProductAsync(int productId, int brandId);
        Task<Brand> AddBrandAsync(Brand brand);
        Task PartiallyUpdateBrandAsync(JsonPatchDocument<UpdateBrandDto> dto, Brand brand);
        Task UpdateBrandAsync(Brand brand);
        Task DeleteBrandAsync(Brand brand);
    }
}
