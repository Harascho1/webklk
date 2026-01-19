namespace WebTemplate.Models;

public class Dostava
{
    [Key]
    public int Id { get; set; }
    public required Materijal Materijal { get; set; }
    public required Stovariste Stovariste { get; set; }
    public int Kolicina { get; set; }
    public required DateTime DatumDostave { get; set; }
}
