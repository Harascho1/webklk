namespace WebTemplate.Models;

public class Korisnik
{
    [Key]
    public int Id { get; set; }

    [Length(2, 20)]
    public required string Ime { get; set; }

    [Length(2, 20)]
    public required string Prezime { get; set; }

    [Length(13, 13)]
    public required string JMBG { get; set; }

    public required string Email { get; set; }

    public required string Sifra { get; set; }
}
