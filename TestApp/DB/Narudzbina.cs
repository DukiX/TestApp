using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TestApp.DB
{
    public class Narudzbina
    {
        [Key]
        public Guid Id { get; set; }
        public List<ElementKorpe> ListaElemenata { get; set; }
        public ApplicationUser Kupac { get; set; }
        public ApplicationUser Prodavac { get; set; }
        public StatusNarudzbine StatusNarudzbine { get; set; }
        public int? VremeIsporukeUDanima { get; set; }
        public DateTime? DatumNarudzbine { get; set; }
        public DateTime? DatumPotvrdeNarudzbine { get; set; }
    }
}
