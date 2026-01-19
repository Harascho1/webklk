using System.Text.Json.Serialization;

namespace WebTemplate.Models;

public class Stovariste
{
    [Key]
    public int Id { get; set; }

    public required string Ime { get; set; }

    public required string Adresa { get; set; }

    [StringLength(11, MinimumLength = 10)]
    public string? BrojTelefona { get; set; }

    [JsonIgnore]
    public List<Dostava>? Dostave { get; set; }
}
