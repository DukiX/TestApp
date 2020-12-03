namespace TestApp.DB
{
    public class Proizvod
    {
        public int Id { get; set; }
        public string Naziv { get; set; }
        public double Cena { get; set; }
        public string Opis { get; set; }
        public string NacinKoriscenja { get; set; }
        public ApplicationUser Prodavac { get; set; }
    }
}
