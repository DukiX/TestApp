using System;
using System.ComponentModel.DataAnnotations;

namespace TestApp.DB
{
    public class ElementKorpe
    {
        [Key]
        public Guid Id { get; set; }
        public Proizvod Proizvod { get; set; }
        public int Kolicina { get; set; }
    }
}
