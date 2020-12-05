using System;
using System.ComponentModel.DataAnnotations;

namespace TestApp.DB
{
    public class Proizvod
    {
        [Key]
        public Guid Id { get; set; }
        public string Naziv { get; set; }
        public double Cena { get; set; }
        public string Opis { get; set; }
        public string NacinKoriscenja { get; set; }
        public ApplicationUser Prodavac { get; set; }
    }
}
