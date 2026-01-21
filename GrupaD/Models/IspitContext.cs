namespace WebTemplate.Models;

public class IspitContext(DbContextOptions options) : DbContext(options)
{
    // DbSet kolekcije!
    public required DbSet<Korisnik> Korisnici { get; set; }
    public required DbSet<Aplikacija> Aplikacije { get; set; }
    public required DbSet<Preplata> Preplate { get; set; }
}
