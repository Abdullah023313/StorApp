using AutoMapper;
using StorApp.Dtos;
using StorApp.Model;

namespace StorApp.Profiles
{
    public class ProductsProfile :Profile
    {
        public ProductsProfile()
        {
            CreateMap<Product, ProductWithoutBrands>()
                .ForMember(dest => dest.Price, source => source.MapFrom(
                    s => s.Price != 0 ? ((int)(s.Price *1.12)).ToString() : "Free"
                    ))
                .ForMember(dest=>dest.Name,source=>source.MapFrom(
                    source=>source.Name.ToUpper()));
            CreateMap<Product, UpdateProductDto>();
        }
    }
}
