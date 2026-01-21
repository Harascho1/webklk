using System.Text.Json.Serialization;

namespace WebTemplate.Models;

public class Aplikacija
{
    [Key]
    public int Id { get; set; }

    [Length(2, 20)]
    public required string Naziv { get; set; }

    [Length(2, 20)]
    public required string ImeProizvodjaca { get; set; }

    public DateTime? DatumIzdavanja { get; set; }

    public double MesecnaPreplata { get; set; }

    [JsonIgnore]
    public List<Preplata>? Preplate { get; set; }
}
