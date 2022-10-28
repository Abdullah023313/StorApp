using Microsoft.AspNetCore.JsonPatch;
using StorApp.Dtos;
using StorApp.Model;

namespace StorApp.Services
{
    public interface IProductsRepository
    {


        Task<IList<Brand>?> GetBrandsAsync();
        Task<(IList<Product>?, PaginationMetaData)> GetProductsAsync(int pageNumber, int pageSize, string name, int? maxPrice, int minPrice);
        Task<Product?> GetProductAsync(int productId, bool includeBrands = false);
        Task<Product> AddProductAsync(Product product);
        Task PartiallyUpdateProductAsync(JsonPatchDocument<ProductDto> dto, Product product);
        Task UpdateProductAsync(Product product);
        Task DeleteAsync(Product product);

    }
}