using System.Runtime.Serialization;

namespace WebTemplate.Controllers;

[ApiController]
[Route("[controller]")]
public class IspitController(IspitContext context) : ControllerBase
{
    public IspitContext Context { get; set; } = context;

    [HttpPost("DodajBiblioteku")]
    public async Task<ActionResult> DodajBiblioteku([FromBody] Biblioteka biblioteka)
    {
        try
        {
            await Context.Biblioteke.AddAsync(biblioteka);
            await Context.SaveChangesAsync();
            return Ok($"Dodaj je {biblioteka.Ime}");
        }
        catch (Exception e)
        {
            return BadRequest(e.ToString());
        }
    }

    [HttpPost("DodajKnjigu/{bibliotekaId}")]
    public async Task<ActionResult> DodajKnjigu([FromBody] Knjiga knjiga, int bibliotekaId)
    {
        try
        {
            var biblioteka = await Context.Biblioteke.FindAsync(bibliotekaId);
            if (biblioteka == null)
            {
                return BadRequest($"Ne postoji biblioteka sa Id-jem: {bibliotekaId}");
            }

            biblioteka.ListKnjiga ??= [];
            biblioteka.ListKnjiga.Add(knjiga);
            Context.Biblioteke.Update(biblioteka);

            knjiga.Biblioteka = biblioteka;

            await Context.Knjige.AddAsync(knjiga);
            await Context.SaveChangesAsync();

            return Ok($"Dodaj je {knjiga.Naslov}");
        }
        catch (Exception e)
        {
            return BadRequest(e.ToString());
        }
    }

    [HttpPost("Iznajmi/{knjigaId}")]
    public async Task<ActionResult> Iznajmi(int knjigaId)
    {
        try
        {
            var knjiga = await Context
                .Knjige.Include(k => k.Biblioteka)
                .Where(k => k.Id == knjigaId)
                .FirstOrDefaultAsync();
            if (knjiga == null)
            {
                return BadRequest("Knjiga ne postoji");
            }

            // Ovo ako ne moze da se iznajmi izdata knjiga
            // dodatna glupa logika
            // if (
            //     await Context
            //         .Izdavanja.Where(i =>
            //             i.Knjiga == knjiga && i.DatumVracanja == DateTime.Parse("3000-1-1")
            //         )
            //         .FirstOrDefaultAsync() != null
            // )
            // {
            //     return BadRequest("Knjiga je vec izdata");
            // }

            var biblioteka = knjiga.Biblioteka;
            if (biblioteka == null)
            {
                return BadRequest("Knjiga nema biblioteku");
            }

            var Iznajmi = new Izdavanje
            {
                Biblioteka = biblioteka,
                Knjiga = knjiga,
                DatumIzdavanja = DateTime.Now,
                DatumVracanja = DateTime.Parse("3000-1-1"),
            };

            await Context.Izdavanja.AddAsync(Iznajmi);
            await Context.SaveChangesAsync();
            return Ok("Proslo");
        }
        catch (Exception e)
        {
            return BadRequest(e.ToString());
        }
    }

    [HttpPut("VratiKnjigu/{knjigaId}/{bibliotekaId}")]
    public async Task<ActionResult> VratiKnjigu(int knjigaId, int bibliotekaId)
    {
        try
        {
            var izdavanje = await Context
                .Izdavanja.Where(i => i.Knjiga.Id == knjigaId && i.Biblioteka.Id == bibliotekaId)
                .FirstOrDefaultAsync();

            if (izdavanje == null)
            {
                return BadRequest("Ne postoji");
            }

            izdavanje.DatumVracanja = DateTime.Now;
            Context.Izdavanja.Update(izdavanje);
            await Context.SaveChangesAsync();
            return Ok("Proslo");
        }
        catch (Exception e)
        {
            return BadRequest(e.ToString());
        }
    }

    [HttpGet("VratiBrojIzdatihKnjiga")]
    public async Task<ActionResult> VratiBrojIzdatihKnjiga()
    {
        try
        {
            //var brojIzdatihKnjiga = await Context.Izdavanja.CountAsync();
            var brojIzdatihKnjiga = await Context
                .Izdavanja.Where(i => i.DatumVracanja != DateTime.Parse("3000-1-1"))
                .CountAsync();

            return Ok($"Broj izdatih knjiga je ${brojIzdatihKnjiga}");
        }
        catch (Exception e)
        {
            return BadRequest(e.ToString());
        }
    }

    [HttpGet("VratiNajcitanijegAutora")]
    public async Task<ActionResult> VratiNajcitanijegAutora()
    {
        try
        {
            var najcitani = await Context
                .Izdavanja.GroupBy(i => i.Knjiga.Autor)
                .Select(grupa => new { ImeAutora = grupa.Key, brojIzdavanja = grupa.Count() })
                .OrderByDescending(x => x.brojIzdavanja)
                .FirstOrDefaultAsync();

            if (najcitani == null)
            {
                return BadRequest("Ne postoji ni jedan Autor");
            }

            return Ok(najcitani);
        }
        catch (Exception e)
        {
            return BadRequest(e.ToString());
        }
    }
}
