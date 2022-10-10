using System.ComponentModel.DataAnnotations;

namespace StorApp.Dtos
{
    public class CreateProductDto
    {

        [Required(ErrorMessage = ":)")]
        public string Name { get; set; } = null!;
        public string Description { get; set; } = "";
        public int? Price { get; set; }
        public int? Amount { get; set; }
    }
}
