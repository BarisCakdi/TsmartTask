using System.ComponentModel.DataAnnotations;

namespace TsmartTask.DTOs
{
    public class dtoProductAdd
    {
        [StringLength(100, ErrorMessage = "İsim 100 karakterden fazla olamaz")]
        public string? Name { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Pozitif sayı girmelisiniz")]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Stock (-)eksi olamaz")]
        public int Stock { get; set; }
    }
}
