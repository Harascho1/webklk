namespace WebTemplate.Models;

public class IspitContext(DbContextOptions options) : DbContext(options)
{
    // DbSet kolekcije!

    public required DbSet<Materijal> Materijali { get; set; }
    public required DbSet<Stovariste> Stovarista { get; set; }
    public required DbSet<Dostava> Dostave { get; set; }
    public required DbSet<Prodaja> Prodaje { get; set; }
}
