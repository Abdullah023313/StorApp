using AutoMapper;
using StorApp.Dtos;
using StorApp.Model;

namespace StorApp.Profiles
{
    public class ProductsProfile :Profile
    {
        public ProductsProfile()
        {
            CreateMap<Product, ProductWithoutBrands>();
        }
    }
}
