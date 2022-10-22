using StorApp.Dtos;

namespace StorApp.Model.Dtos
{
    public class ProductWithBrands
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int Price { get; set; }
        public int Amount { get; set; }
        public List<BrandDto> Brands { get; set; } = new List<BrandDto>();
    }
}
