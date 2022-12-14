using System.Reflection.Metadata.Ecma335;

namespace StorApp.Model
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int Price { get; set; }
        public int Amount { get; set; }
        public bool IsDeleted { get; set; }= false!;       
        public List<Brand> Brands { get; set; } = new List<Brand>();

    }
   
}