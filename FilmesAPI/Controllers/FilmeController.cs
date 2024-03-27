using AutoMapper;
using FilmesAPI.Data;
using FilmesAPI.Data.Dtos;
using FilmesAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace FilmesAPI.Controllers;

[ApiController]
[Route("[controller]")]

public class FilmeController : ControllerBase
{
    private FilmeContext _context;
    private IMapper _mapper;

    public FilmeController(FilmeContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpPost]                                                  //Sempre que fizer uma operação post para este controlador, irá executar o que estiver abaixo
    public IActionResult AdicionaFilme([FromBody] CreateFilmeDto filmeDto)  //[FromBody] = indica da onde pegará a informação (do corpo)
    {
        Filme filme = _mapper.Map<Filme>(filmeDto);
        _context.Filmes.Add(filme);
        _context.SaveChanges();                                 //comando para confirmar e salvar as alterações
        return CreatedAtAction(nameof(RecuperaFilmePorId), 
            new { id = filme.Id }, filme);                      // serve para retornar ao consumidor as informações, através do ID
    }

    [HttpGet]
    public IEnumerable<Filme> RecuperarFilmes([FromQuery] int skip = 0, [FromQuery] int take = 50)
    {
        return _context.Filmes.Skip(skip).Take(take);
    }

    [HttpGet("{id}")]                                           // para diferenciar o get, informamos que precisamos receber
    public IActionResult RecuperaFilmePorId(int id)             //IActionResult serve para definir que irá trazer um resultado, seja positivo ou negativo
    {
        var filme = _context.Filmes.FirstOrDefault(filme => filme.Id == id);
        if (filme == null) return NotFound();                   // Padronizado estilo Rest
        return Ok(filme);
    }

    [HttpPut ("{id}")]
    public IActionResult AtualizaFilme(int id, [FromBody] UpdateFilmeDto filmeDto)
    {
        var filme = _context.Filmes.FirstOrDefault(filme => filme.Id == id);
        if (filme == null) return NotFound();
        _mapper.Map(filmeDto, filme);
        _context.SaveChanges();
        return NoContent();
    }
}
