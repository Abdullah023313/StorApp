namespace StorApp.Model
{
    public class Brand
    {
        public int BrandId { get; set; }
        public string Name { get; set; } = "";
        public int ProductId { get; set; }
        public Product Product { get; set; } = new Product();

    }
}