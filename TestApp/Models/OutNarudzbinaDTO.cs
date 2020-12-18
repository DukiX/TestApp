using System;
using System.Collections.Generic;

namespace TestApp.Models
{
    public class OutNarudzbinaDTO
    {
        public Guid Id { get; set; }
        public Account Prodavac { get; set; }
        public List<OutElementKorpeDTO> ListaElemenata { get; set; }
    }
}
