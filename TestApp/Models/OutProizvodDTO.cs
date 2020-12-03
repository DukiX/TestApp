namespace TestApp.Models
{
    public class OutProizvodDTO
    {
        public string Naziv { get; set; }
        public double Cena { get; set; }
        public string Opis { get; set; }
        public string NacinKoriscenja { get; set; }
        public Account Prodavac { get; set; }
    }
}
