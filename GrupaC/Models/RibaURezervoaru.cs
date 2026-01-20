namespace WebTemplate.Models;

public class RibaURezervoaru
{
    public int Id { get; set; }

    public required Riba Riba { get; set; }
    public required Rezervoar Rezervoar { get; set; }

    public int BrojJedinki { get; set; }

    public DateTime DatumDodavanja { get; set; }
}
