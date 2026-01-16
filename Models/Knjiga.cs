public class Knjiga
{
    [Key]
    public int Id { get; set; }
    public required string Naslov { get; set; }
    public required string Autor { get; set; }
    public int GodinaIzdavanja { get; set; }
    public required string Izdavac { get; set; }
    public int EvidecioniBroj { get; set; }
    public Biblioteka? Biblioteka { get; set; }
}
