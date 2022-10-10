using Microsoft.AspNetCore.JsonPatch;
using StorApp.Dtos;
using StorApp.Model;

namespace StorApp.Services
{
    public interface IProductsService
    {
        Product Add(Product product);
        void Delete(Product product);
        IList<Product> GetAll();
        Product GetById(int id);
        void PartiallyUpdate(JsonPatchDocument<UpdateProductDto> dto, Product product);
        void Update(Product product);
    }
}