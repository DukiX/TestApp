using System;
using TestApp.Enums;

namespace TestApp.Models
{
    public class OutProizvodDTO
    {
        public Guid Id { get; set; }
        public string Naziv { get; set; }
        public double Cena { get; set; }
        public string Opis { get; set; }
        //[JsonConverter(typeof(JsonStringEnumConverter))]
        public NacinKoriscenja? NacinKoriscenja { get; set; }
        public Account Prodavac { get; set; }
        public string Slika { get; set; }
    }
}
