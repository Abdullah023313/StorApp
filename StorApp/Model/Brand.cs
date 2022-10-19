namespace StorApp.Model
{
    public class Brand
    {
        public int BrandId { get; set; }
        public string Name { get; set; } = null!;
        public string Notes { get; set; } = null!;
        public int ProductId { get; set; }

    }
}