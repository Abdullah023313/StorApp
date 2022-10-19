using System.ComponentModel.DataAnnotations;

namespace StorApp.Dtos
{
    public class ProductWithoutBrands
    { 
        public string Name { get; set; } = null!;
        public string Description { get; set; }=null!;
        public string Price { get; set; }=null!;
        public int Amount { get; set; }
    }
}
