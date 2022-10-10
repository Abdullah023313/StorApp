using Microsoft.AspNetCore.JsonPatch;
using StorApp.Dtos;
using StorApp.Model;

namespace StorApp.Services
{
    public interface IProductsService
    {
        Task<Product> AddAsync(Product product);
        void DeleteAsync(Product product);
        Task<IList<Product>?> GetAllAsync();
        Task<Product?> GetByIdAsync(int id);
        void PartiallyUpdateAsync(JsonPatchDocument<UpdateProductDto> dto, Product product);
        void UpdateAsync(Product product);
    }
}