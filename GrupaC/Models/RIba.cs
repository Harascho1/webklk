namespace WebTemplate.Models;

public class Riba
{
    [Key]
    public int Id { get; set; }

    [MinLength(3)]
    public required string NazivVrste { get; set; }

    [Range(1, 2000)]
    public double Masa { get; set; }

    [Range(0, 50)]
    public int GodineStarosti { get; set; }
}
