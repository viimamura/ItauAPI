using Microsoft.AspNetCore.Mvc;
using ItauAPI.Models;
using Microsoft.Extensions.Caching.Memory;

namespace ItauAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaisesController : ControllerBase
{
    private readonly IMemoryCache _cache;

    public PaisesController(IMemoryCache cache)
    {
        _cache = cache;
    }

    private IList<Pais> ObterPaisesExistentes()
    {
        return new List<Pais>
        {
            new Pais { Nome = "Chile", CodigoISO = "56", Continente = "América", Populacao = 16746491 },
            new Pais { Nome = "Guiné-Bissau", CodigoISO = "245", Continente = "África", Populacao = 1565126 },
            new Pais { Nome = "Austrália", CodigoISO = "61", Continente = "Oceania", Populacao = 21515754 },
            new Pais { Nome = "Coreia do Sul", CodigoISO = "82", Continente = "Ásia", Populacao = 48422644 },
            new Pais { Nome = "Vaticano", CodigoISO = "379", Continente = "Europa", Populacao = 921 }
        };
    }

    [HttpGet]
    public ActionResult<IList<Pais>> ListarPaises()
    {
        if (_cache.TryGetValue("paises", out IList<Pais> paises))
        {
            return Ok(paises);
        }

        paises = ObterPaisesExistentes();

        var cacheEntryOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        };

        _cache.Set("paises", paises, cacheEntryOptions);

        return Ok(paises);
    }

    [HttpGet("{codigoISO}")]
    public ActionResult<IList<Pais>> ConsultarPaisPeloISO(string codigoISO)
    {
        if (_cache.TryGetValue("paises", out IList<Pais> paises))
        {
            var pais = paises.FirstOrDefault(p => p.CodigoISO == codigoISO);

            if (pais != null)
            {
                return Ok(pais);
            }
        }

        return NotFound();
    }

    [HttpPost]
    public ActionResult<IList<Pais>> AdicionarPais([FromBody] Pais pais)
    {
        //Cria a lista pais a partir dos dados do cache ou uma lista vazia
        var paises = _cache.TryGetValue("paises", out IList<Pais> dadosPaises) ? dadosPaises.ToList() : new List<Pais>();

        //Adicionando o novo pais na lista
        paises.Add(pais);

        //Atualize os dados em cache
        var cacheEntryOptions = new MemoryCacheEntryOptions
        {
            //Os dados em cache expiram em 5 minutos
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        };

        _cache.Set("paises", paises, cacheEntryOptions);

        // Retorna o novo país com o ID atribuído
        return CreatedAtAction(nameof(ObterPaisesExistentes), new { codigoISO = pais.CodigoISO }, pais);
    }
}