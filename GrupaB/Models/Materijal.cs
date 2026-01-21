using System.Text.Json.Serialization;

namespace WebTemplate.Models;

public class Materijal
{
    [Key]
    public int Id { get; set; }
    public required string Sifra { get; set; }

    public required string Naziv { get; set; }

    public double Cena { get; set; }

    public string? NazivProizvodjaca { get; set; }

    [JsonIgnore]
    public List<Dostava>? Dostave { get; set; }
}
