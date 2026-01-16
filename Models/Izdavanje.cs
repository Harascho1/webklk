namespace WebTemplate.Models;

public class Izdavanje
{
    [Key]
    public int Id { get; set; }
    public required Biblioteka Biblioteka { get; set; }
    public required Knjiga Knjiga { get; set; }
    public required DateTime DatumIzdavanja { get; set; }
    public required DateTime DatumVracanja { get; set; }
}
