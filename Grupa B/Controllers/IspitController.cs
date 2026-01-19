namespace WebTemplate.Controllers;

[ApiController]
[Route("[controller]")]
public class IspitController(IspitContext context) : ControllerBase
{
    public IspitContext Context { get; set; } = context;

    [HttpPost("DodajMaterijal")]
    public async Task<ActionResult> DodajMaterijal([FromBody] Materijal materijal)
    {
        try
        {
            await Context.Materijali.AddAsync(materijal);
            await Context.SaveChangesAsync();
            return Ok(materijal);
        }
        catch (Exception e)
        {
            return BadRequest(e.ToString());
        }
    }

    [HttpPost("DodajStovariste")]
    public async Task<ActionResult> DodajStovariste([FromBody] Stovariste stovariste)
    {
        try
        {
            await Context.Stovarista.AddAsync(stovariste);
            await Context.SaveChangesAsync();
            return Ok(stovariste);
        }
        catch (Exception e)
        {
            return BadRequest(e.ToString());
        }
    }

    [HttpPost("PrijemMaterijala")]
    public async Task<ActionResult> PrijemMaterijala([FromBody] PrijemMaterijala prijemMaterijala)
    {
        try
        {
            var stovariste = await Context.Stovarista.FindAsync(prijemMaterijala.StovaristeId);
            if (stovariste == null)
            {
                return BadRequest($"Stovariste sa id:{prijemMaterijala.StovaristeId} ne postoji");
            }

            var materijal = await Context.Materijali.FindAsync(prijemMaterijala.MaterijalId);
            if (materijal == null)
            {
                return BadRequest($"Materijal sa id:{prijemMaterijala.MaterijalId} ne postoji");
            }

            var dostava = new Dostava
            {
                Stovariste = stovariste,
                Materijal = materijal,
                DatumDostave = DateTime.Now,
                Kolicina = prijemMaterijala.Kolicina,
            };

            await Context.Dostave.AddAsync(dostava);

            stovariste.Dostave ??= [];
            stovariste.Dostave.Add(dostava);
            Context.Stovarista.Update(stovariste);

            materijal.Dostave ??= [];
            materijal.Dostave.Add(dostava);
            Context.Materijali.Update(materijal);

            await Context.SaveChangesAsync();
            return Ok(dostava);
        }
        catch (Exception e)
        {
            return BadRequest(e.ToString());
        }
    }

    [HttpGet("UkupnaKolicina/{stovaristeId}")]
    public async Task<ActionResult> UkupnaKolicina(int stovaristeId)
    {
        try
        {
            return Ok(
                await Context
                    .Dostave.Where(d => d.Stovariste.Id == stovaristeId)
                    .SumAsync(k => k.Kolicina)
            );

            // Ovako isto moze ali nema potrebe za komplikovanje
            // Fora je sto ova funkcija vraca tip Task<int> a ako nama treba iskljucivo
            // int onda trebamo da uzmemo Result task-a sto je int
            // Task<int>.Result -> int
            // return Ok(await Context.Dostave
            //      .Where(d => d.Stovariste.Id == stovaristeId)
            //      .SumAsync(k => k.Kolicina.Result));
        }
        catch (Exception e)
        {
            return BadRequest(e.ToString());
        }
    }

    [HttpPost("Prodaj")]
    public async Task<ActionResult> Prodaj([FromBody] PrijemMaterijala prijemMaterijala)
    {
        try
        {
            var dostava = await Context
                .Dostave.Where(d =>
                    d.Materijal.Id == prijemMaterijala.MaterijalId
                    && d.Stovariste.Id == prijemMaterijala.StovaristeId
                    && d.Kolicina >= prijemMaterijala.Kolicina
                )
                .FirstOrDefaultAsync();
            if (dostava == null)
            {
                return BadRequest("Ne postoji takva dostava");
            }

            var prodaja = new Prodaja
            {
                Dostava = dostava,
                DatumProdaje = DateTime.Now,
                Kolicina = prijemMaterijala.Kolicina,
            };
            await Context.Prodaje.AddAsync(prodaja);

            dostava.Kolicina -= prijemMaterijala.Kolicina;
            Context.Dostave.Update(dostava);

            await Context.SaveChangesAsync();
            return Ok(prodaja);
        }
        catch (Exception e)
        {
            return BadRequest(e.ToString());
        }
    }

    [HttpGet("Materijal/{stovaristeIme}")]
    public async Task<ActionResult> Materijal(string stovaristeIme)
    {
        try
        {
            return Ok(
                await Context
                    .Dostave.Where(d => d.Stovariste.Ime == stovaristeIme)
                    .GroupBy(d => new { d.Materijal.Id, d.Materijal.Naziv })
                    .Select(grupa => new
                    {
                        Naziv = grupa.Key.Naziv,
                        Kolicina = grupa.Sum(k => k.Kolicina),
                    })
                    .OrderByDescending(x => x.Kolicina)
                    .FirstOrDefaultAsync()
            );
            /*
            Razbijamo upit:

            1. Where()
               Filtriramo dostave za stovarište koje ima isto ime kao prosleni parametar.

            2. GroupBy()
               Grupisemo dostave po Materijal.Id i Materijal.Naziv.
               Koristimo i Id i Naziv da bismo izbegli situaciju da različiti materijali
               sa istim nazivom budu spojeni u jednu grupu.

            3. Select()
               Iz svake grupe biramo:
               - Naziv materijala
               - Ukupnu količinu tog materijala (zbir svih dostava u grupi)

               Sum() računa ukupnu količinu sabiranjem Kolicina
               svih Dostava objekata u jednoj grupi.

            4. OrderByDescending()
               Sortiramo rezultate opadajuće po količini.

            5. FirstOrDefaultAsync()
               Vraćamo materijal sa najvećom ukupnom količinom
               (ili null ako nema rezultata).
            */
        }
        catch (Exception e)
        {
            return BadRequest(e.ToString());
        }
    }
}
