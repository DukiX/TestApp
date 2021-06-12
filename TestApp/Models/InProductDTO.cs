using System.ComponentModel.DataAnnotations;
using TestApp.Enums;

namespace TestApp.Models
{
    public class InProductDTO
    {
        [Required]
        public string Naziv { get; set; }
        [Required]
        public double Cena { get; set; }
        [Required]
        public string Opis { get; set; }
        [Required]
        public NacinKoriscenja? NacinKoriscenja { get; set; }
    }
}
