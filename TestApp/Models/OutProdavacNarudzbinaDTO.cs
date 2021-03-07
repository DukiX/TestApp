using System;
using System.Collections.Generic;
using TestApp.Enums;

namespace TestApp.Models
{
    public class OutProdavacNarudzbinaDTO
    {
        public Guid Id { get; set; }
        public List<OutElementKorpeDTO> ListaElemenata { get; set; }
        public Account Prodavac { get; set; }
        public Account Kupac { get; set; }
        public StatusNarudzbine StatusNarudzbine { get; set; }
        public int? VremeIsporukeUDanima { get; set; }
        public DateTime? DatumNarudzbine { get; set; }
        public DateTime? DatumOdobrenjaNarudzbine { get; set; }
    }
}
