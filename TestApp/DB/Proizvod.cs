using System;
using System.ComponentModel.DataAnnotations;
using TestApp.Enums;

namespace TestApp.DB
{
    public class Proizvod
    {
        [Key]
        public Guid Id { get; set; }
        public string Naziv { get; set; }
        public double Cena { get; set; }
        public string Opis { get; set; }
        public NacinKoriscenja? NacinKoriscenja { get; set; }
        public ApplicationUser Prodavac { get; set; }
    }
}
