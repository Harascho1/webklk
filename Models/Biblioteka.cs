namespace WebTemplate.Models;

public class Biblioteka
{
    [Key]
    public int Id { get; set; }
    public required string Ime { get; set; }
    public required string Adresa { get; set; }
    public required string Email { get; set; }
    public List<Knjiga>? ListKnjiga { get; set; }
}
