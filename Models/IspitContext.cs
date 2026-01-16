namespace WebTemplate.Models;

public class IspitContext(DbContextOptions options) : DbContext(options)
{
    // DbSet kolekcije!
    public required DbSet<Knjiga> Knjige { get; set; }
    public required DbSet<Biblioteka> Biblioteke { get; set; }
    public required DbSet<Izdavanje> Izdavanja { get; set; }
}
