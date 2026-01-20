using System.Text.Json.Serialization;

namespace WebTemplate.Models;

public class Rezervoar
{
    [Key]
    [Length(minimumLength: 6, maximumLength: 6)]
    public required string Sifra { get; set; }

    [Range(1.00, double.MaxValue)]
    public double Zapremina { get; set; }

    [Range(-10.00, 30.00)]
    public double Temperatura { get; set; }
    public DateOnly DatumPoslednjegCiscenja { get; set; }

    [Range(1, 20)]
    public int Kapacitet { get; set; }

    [JsonIgnore]
    public List<RibaURezervoaru>? Ribice { get; set; }
    public int FrekvencijaCiscenja { get; set; }
}
