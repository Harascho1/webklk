namespace WebTemplate.Models;

public class Preplata
{
    [Key]
    public int Id { get; set; }

    [Length(15, 15)]
    public required string KljucPreplate { get; set; }

    public required Korisnik Korisnik { get; set; }
    public required Aplikacija Aplikacija { get; set; }

    public required DateTime DatumPreplate { get; set; }

    public int BrojPreplacenihMeseci { get; set; }

    public required DateTime DatumIsteka { get; set; }
}
