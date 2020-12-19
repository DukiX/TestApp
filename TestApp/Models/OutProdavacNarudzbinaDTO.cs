using System.Collections.Generic;
using TestApp.DB;

namespace TestApp.Models
{
    public class OutProdavacNarudzbinaDTO
    {
        public List<OutElementKorpeDTO> ListaElemenata { get; set; }
        public Account Kupac { get; set; }
        public StatusNarudzbine StatusNarudzbine { get; set; }
        public int? VremeIsporukeUDanima { get; set; }
    }
}
