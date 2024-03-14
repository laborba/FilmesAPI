using FilmesAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace FilmesAPI.Controllers;

[ApiController]
[Route("[controller]")]

public class FilmeController : ControllerBase
{
    private static List<Filme> filmes = new List<Filme>();
    private static int id = 0;


    [HttpPost]                                                  //Sempre que fizer uma operação post para este controlador, irá executar o que estiver abaixo
    public IActionResult AdicionaFilme([FromBody] Filme filme)  //[FromBody] = indica da onde pegará a informação (do corpo)
    {
        filme.Id = id++;
        filmes.Add(filme);
        return CreatedAtAction(nameof(RecuperaFilmePorId), 
            new { id = filme.Id }, filme);                      // serve para retornar ao consumidor as informações, através do ID
    }

    [HttpGet]
    public IEnumerable<Filme> RecuperarFilmes([FromQuery] int skip = 0, [FromQuery] int take = 50)
    {
        return filmes.Skip(skip).Take(take);
    }

    [HttpGet("{id}")]                                           // para diferenciar o get, informamos que precisamos receber
    public IActionResult RecuperaFilmePorId(int id)             //IActionResult serve para definir que irá trazer um resultado, seja positivo ou negativo
    {
        var filme = filmes.FirstOrDefault(filme => filme.Id == id);
        if (filme == null) return NotFound();                   // Padronizado estilo Rest
        return Ok(filme);
    }
}
