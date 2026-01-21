namespace WebTemplate.Models;

public class Prodaja
{
    public int Id { get; set; }

    public required Dostava Dostava { get; set; }

    public DateTime DatumProdaje { get; set; }
    public int Kolicina { get; set; }
}
