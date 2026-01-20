using Microsoft.Identity.Client;
using WebTemplate.Codes;

namespace WebTemplate.Controllers;

[ApiController]
[Route("[controller]")]
public class IspitController(IspitContext context) : ControllerBase
{
    public IspitContext Context { get; set; } = context;

    [HttpPost("dodaj-ribu")]
    public async Task<ActionResult> DodajRibu([FromBody] Riba riba)
    {
        try
        {
            await Context.Ribe.AddAsync(riba);
            await Context.SaveChangesAsync();
            return Ok(riba);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("dodaj-rezervoar")]
    public async Task<ActionResult> DodajRezervoar([FromBody] Rezervoar rezervoar)
    {
        try
        {
            await Context.Rezervoari.AddAsync(rezervoar);
            await Context.SaveChangesAsync();
            return Ok(rezervoar);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("dodaj-ribu-u-rezervoar")]
    public async Task<ActionResult> DodajRibuURezervoar([FromBody] DodajRibu dodajRibu)
    {
        try
        {
            var riba = await Context.Ribe.FindAsync(dodajRibu.RibaId);
            if (riba == null)
            {
                return BadRequest($"Riba sa id-em:{dodajRibu.RibaId} ne postoji");
            }

            var rezervoar = await Context.Rezervoari.FindAsync(dodajRibu.RezervoarSifra);
            if (rezervoar == null)
            {
                return BadRequest($"Rezervoar sa sifrom:${dodajRibu.RezervoarSifra} ne postoji");
            }

            var rezervoarKapacitet = await Context
                .RibeURezervoarima.Where(r => r.Rezervoar == rezervoar)
                .SumAsync(r => r.BrojJedinki);
            if (rezervoar.Kapacitet - rezervoarKapacitet - dodajRibu.BrojJedinki < 0)
            {
                return BadRequest("Rezervoar nema dovoljan kapacitet");
            }

            bool postojiKonflikt = await Context
                .RibeURezervoarima.Where(s => s.Rezervoar.Sifra == rezervoar.Sifra)
                .AnyAsync(r => (r.Riba.Masa >= riba.Masa * 10) || (riba.Masa >= r.Riba.Masa * 10));

            if (postojiKonflikt)
            {
                return BadRequest("Postoje ribe u akvirujumu koje bi pojele ove ribe/ribu");
            }

            var novo = new RibaURezervoaru
            {
                Riba = riba,
                Rezervoar = rezervoar,
                DatumDodavanja = DateTime.Now,
                BrojJedinki = dodajRibu.BrojJedinki,
            };
            await Context.RibeURezervoarima.AddAsync(novo);

            rezervoar.Ribice ??= [];
            rezervoar.Ribice.Add(novo);
            Context.Rezervoari.Update(rezervoar);

            await Context.SaveChangesAsync();
            return Ok(novo);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut("riba-u-rezervoaru")]
    public async Task<ActionResult> UpdateRibuURazervoaru([FromBody] DodajRibu updateRibu)
    {
        try
        {
            var rezervoar = await Context.Rezervoari.FindAsync(updateRibu.RezervoarSifra);
            if (rezervoar == null)
            {
                return BadRequest($"Riba sa id-em:{updateRibu.RezervoarSifra} ne postoji");
            }

            var ribaURezervoaru = await Context
                .RibeURezervoarima.Where(r =>
                    r.Riba.Id == updateRibu.RibaId && r.Rezervoar.Sifra == updateRibu.RezervoarSifra
                )
                .FirstOrDefaultAsync();
            if (ribaURezervoaru == null)
            {
                return BadRequest($"Rezervoar sa sifrom:${updateRibu.RezervoarSifra} ne postoji");
            }

            var rezervoarKapacitet = await Context
                .RibeURezervoarima.Where(r => r.Rezervoar == rezervoar)
                .SumAsync(r => r.BrojJedinki);

            int noviKapacitet =
                rezervoar.Kapacitet
                - (rezervoarKapacitet - ribaURezervoaru.BrojJedinki + updateRibu.BrojJedinki);
            if (noviKapacitet < 0)
            {
                return BadRequest("Rezervoar nema dovoljan kapacitet");
            }

            ribaURezervoaru.BrojJedinki = updateRibu.BrojJedinki;
            ribaURezervoaru.DatumDodavanja = DateTime.Now;

            Context.RibeURezervoarima.Update(ribaURezervoaru);

            await Context.SaveChangesAsync();
            return Ok(ribaURezervoaru);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("treba-ocistiti")]
    public async Task<ActionResult> PronaciSveRezervoareKojeTrebaOcistiti()
    {
        try
        {
            var listaRezervoara = await Context
                .Rezervoari.Where(r =>
                    r.DatumPoslednjegCiscenja.AddDays(r.FrekvencijaCiscenja)
                    <= DateOnly.FromDateTime(DateTime.Now)
                )
                .Select(r => r.Sifra)
                .ToArrayAsync();

            return Ok(listaRezervoara);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("ukupna-masa/{rezervoarSifra}")]
    public async Task<ActionResult> UkupnaMasaRIba(string rezervoarSifra)
    {
        try
        {
            var ukupnaMasa = await Context
                .RibeURezervoarima.Where(r => r.Rezervoar.Sifra == rezervoarSifra)
                .SumAsync(s => s.BrojJedinki * s.Riba.Masa);

            return Ok($"Ukupna masa je: {ukupnaMasa}");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}
