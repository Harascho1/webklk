namespace WebTemplate.Models;

public class IspitContext(DbContextOptions options) : DbContext(options)
{
    // DbSet kolekcije!
    public required DbSet<Riba> Ribe { get; set; }
    public required DbSet<Rezervoar> Rezervoari { get; set; }
    public required DbSet<RibaURezervoaru> RibeURezervoarima { get; set; }
}
