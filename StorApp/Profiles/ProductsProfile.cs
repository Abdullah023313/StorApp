using AutoMapper;
using StorApp.Dtos;
using StorApp.Model;
using StorApp.Model.Dtos;

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
                    s=>s.Name.ToUpper()));


            CreateMap<Product, ProductWithBrands>()
                .ForMember(dest => dest.Price, source => source.MapFrom(
                    s => s.Price != 0 ? ((int)(s.Price * 1.12)).ToString() : "Free"
                    ))
                .ForMember(dest => dest.Name, source => source.MapFrom(
                    s => s.Name.ToUpper()))
                .ForMember(dest => dest.Brands, source => source.MapFrom(
                   s => s.Brands))
                  ;

            CreateMap<ProductDto, Product>();

            CreateMap<Brand, BrandDto>();
              
        }
    }
}
