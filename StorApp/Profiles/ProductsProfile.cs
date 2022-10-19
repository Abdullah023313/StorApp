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
                    s => s.Price != 0 ? (s.Price *1.12).ToString() : "Free"
                    ));
            CreateMap<Product, UpdateProductDto>();
        }
    }
}
