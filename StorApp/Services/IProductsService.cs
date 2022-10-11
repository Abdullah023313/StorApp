using Microsoft.AspNetCore.JsonPatch;
using StorApp.Dtos;
using StorApp.Model;

namespace StorApp.Services
{
    public interface IProductsService
    {
        Task<Product> AddAsync(Product product);
        Task DeleteAsync(Product product);
        Task<IList<Product>?> GetAllAsync();
        Task<Product?> GetByIdAsync(int id);
        Task PartiallyUpdateAsync(JsonPatchDocument<UpdateProductDto> dto, Product product);
        Task UpdateAsync(Product product);
    }
}