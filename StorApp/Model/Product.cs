namespace StorApp.Model
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = "";
        public int? Price { get; set; }
        public int? Amount { get; set; }

        public List<Brand> Brands { get; set; } = new List<Brand>();

    }
}