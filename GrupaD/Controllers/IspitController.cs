namespace WebTemplate.Controllers;

[ApiController]
[Route("[controller]")]
public class IspitController(IspitContext context) : ControllerBase
{
    public IspitContext Context { get; set; } = context;

    [HttpPost("dodaj-korisnika")]
    public async Task<ActionResult> DodajKorisnika([FromBody] Korisnik korisnik)
    {
        try
        {
            await Context.Korisnici.AddAsync(korisnik);
            await Context.SaveChangesAsync();
            return Ok(korisnik);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("dodaj-aplikaciju")]
    public async Task<ActionResult> DodajAplikaciju([FromBody] Aplikacija aplikacija)
    {
        try
        {
            await Context.Aplikacije.AddAsync(aplikacija);
            await Context.SaveChangesAsync();
            return Ok(aplikacija);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("preplata/{aplikacijaId}/{korisnikId}/{brojmeseci}")]
    public async Task<ActionResult> Preplata(int aplikacijaId, int korisnikId, int brojmeseci)
    {
        try
        {
            var korisnik = await Context.Korisnici.FindAsync(korisnikId);
            if (korisnik == null)
            {
                return BadRequest("Nema korisnika");
            }

            var aplikacija = await Context.Aplikacije.FindAsync(aplikacijaId);
            if (aplikacija == null)
            {
                return BadRequest("Nema aplikacije");
            }

            var preplata = new Preplata
            {
                Korisnik = korisnik,
                Aplikacija = aplikacija,
                KljucPreplate = korisnik.JMBG + '0' + korisnik.Id,
                DatumPreplate = DateTime.Now,
                BrojPreplacenihMeseci = brojmeseci,
                DatumIsteka = DateTime.Now.AddMonths(brojmeseci),
            };

            await Context.Preplate.AddAsync(preplata);

            aplikacija.Preplate ??= [];
            aplikacija.Preplate.Add(preplata);
            Context.Aplikacije.Update(aplikacija);

            await Context.SaveChangesAsync();
            return Ok(preplata);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("obnovi/{aplikacijaId}/{korisnikId}/{brojmeseci}")]
    public async Task<ActionResult> Obnovi(int aplikacijaId, int korisnikId, int brojmeseci)
    {
        try
        {
            var preplata = await Context
                .Preplate.Where(p => p.Korisnik.Id == korisnikId && p.Aplikacija.Id == aplikacijaId)
                .FirstOrDefaultAsync();
            if (preplata == null)
            {
                return BadRequest("Nema preplate");
            }

            preplata.DatumPreplate = DateTime.Now;
            preplata.BrojPreplacenihMeseci = brojmeseci;
            preplata.DatumIsteka = DateTime.Now.AddMonths(brojmeseci);

            Context.Preplate.Update(preplata);
            await Context.SaveChangesAsync();
            return Ok(preplata);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("istekle/{aplikacijaId}")]
    public async Task<ActionResult> Istekle(int aplikacijaId)
    {
        try
        {
            return Ok(
                await Context
                    .Preplate.Where(p =>
                        p.Aplikacija.Id == aplikacijaId && p.DatumIsteka < DateTime.Now
                    )
                    .Select(p => p.Korisnik.Id)
                    .Distinct()
                    .CountAsync()
            );
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("profit/{aplikacijaId}")]
    public async Task<ActionResult> Profit(int aplikacijaId)
    {
        try
        {
            return Ok(
                await Context
                    .Preplate.Where(p => p.Aplikacija.Id == aplikacijaId)
                    .SumAsync(p => p.BrojPreplacenihMeseci * p.Aplikacija.MesecnaPreplata)
            );
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}
