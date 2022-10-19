using Microsoft.AspNetCore.JsonPatch;
using StorApp.Dtos;
using StorApp.Model;

namespace StorApp.Services
{
    public interface IProductsRepository
    {
        Task<Product> AddProductAsync(Product product);
        Task DeleteAsync(Product product);
        Task<(IList<Product>?, PaginationMetaData)> GetProductsAsync(int pageNumber, int pageSize, string name, int? maxPrice, int minPrice);
        Task<Product?> GetProductAsync(int productId, bool includeBrands=false);
        Task PartiallyUpdateProductAsync(JsonPatchDocument<UpdateProductDto> dto, Product product);
        Task UpdateProductAsync(Product product);

    }
}