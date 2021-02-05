using System.ComponentModel.DataAnnotations;
using TestApp.Enums;

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
        //[JsonConverter(typeof(JsonStringEnumConverter))]
        public NacinKoriscenja? NacinKoriscenja { get; set; }
    }
}
