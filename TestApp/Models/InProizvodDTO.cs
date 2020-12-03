using System.ComponentModel.DataAnnotations;

namespace TestApp.Models
{
    public class InProizvodDTO
    {
        [Required]
        public string Naziv { get; set; }
        [Required]
        public double Cena { get; set; }
        [Required]
        public string Opis { get; set; }
        [Required]
        public string NacinKoriscenja { get; set; }
    }
}
