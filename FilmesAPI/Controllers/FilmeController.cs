using AutoMapper;
using FilmesAPI.Data;
using FilmesAPI.Data.Dtos;
using FilmesAPI.Models;
using Microsoft.AspNetCore.JsonPatch;
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

    /// <summary>
    /// Adiciona um filme ao banco de dados
    /// </summary>
    /// <param name="filmeDto">Objeto com os campos necessários para criação de um filme</param>
    /// <returns>IActionResult</returns>
    /// <response code="201">Caso inserção seja feita com sucesso</response>
    /// <response code="404">Caso a inserção falhe por falta de informação</response>
    [HttpPost]                                                  //Sempre que fizer uma operação post para este controlador, irá executar o que estiver abaixo
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult AdicionaFilme([FromBody] CreateFilmeDto filmeDto)  //[FromBody] = indica da onde pegará a informação (do corpo)
    {
        Filme filme = _mapper.Map<Filme>(filmeDto);
        _context.Filmes.Add(filme);
        _context.SaveChanges();                                 //comando para confirmar e salvar as alterações
        return CreatedAtAction(nameof(RecuperaFilmePorId),
            new { id = filme.Id }, filme);                      // serve para retornar ao consumidor as informações, através do ID
    }

    /// <summary>
    /// Traz a lista dos filmes existentes no banco de dados
    /// </summary>
    /// <param name="skip">Objeto para especificar quantos filmes da lista pular</param>
    /// <param name="take">Objeto para especificar quantos filmes da lista mostrar</param>
    /// <returns>IActionResult</returns>
    /// <response code="200">Caso a busca seja executada com sucesso</response>
    /// <response code="404">Caso a busca não tenha sucesso</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IEnumerable<ReadFilmeDto> RecuperarFilmes(
        [FromQuery] int skip = 0, 
        [FromQuery] int take = 50, 
        [FromQuery] string? nomeCinema = null)
    {
        if (nomeCinema == null)
        {
            return _mapper.Map<List<ReadFilmeDto>>(_context.Filmes.Skip(skip).Take(take).ToList());
        }
        return _mapper.Map<List<ReadFilmeDto>>(_context.Filmes.Skip(skip).Take(take)
            .Where(filme => filme.Sessoes.Any(sessao => sessao.Cinema.Nome == nomeCinema)).ToList());

    }

    /// <summary>
    /// Traz o filme através do ID passado
    /// </summary>
    /// <param name="id">Objeto com o campo ID necessário para buscar o filme</param>
    /// <returns>IActionResult</returns>
    /// <response code="200">Caso a busca seja executada com sucesso</response>
    /// <response code="404">Caso a busca não tenha sucesso</response>
    [HttpGet("{id}")]                                           // para diferenciar o get, informamos que precisamos receber
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult RecuperaFilmePorId(int id)             //IActionResult serve para definir que irá trazer um resultado, seja positivo ou negativo
    {
        var filme = _context.Filmes.FirstOrDefault(filme => filme.Id == id);
        if (filme == null) return NotFound();                   // Padronizado estilo Rest
        var filmeDto = _mapper.Map<ReadFilmeDto>(filme);        
        return Ok(filmeDto);
    }

    /// <summary>
    /// Atualizar todas as informações de um filme passado pelo ID
    /// </summary>
    /// <param name="id">Objeto com o campo ID necessário para buscar o filme</param>
    /// <param name="filmeDto">Objeto com os campos necessários para atualização de um filme</param>
    /// <returns>IActionResult</returns>
    /// <response code="204">Caso a atualização seja executada com sucesso</response>
    /// <response code="404">Caso a busca ou atualização não tenham sucesso</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult AtualizaFilme(int id, [FromBody] UpdateFilmeDto filmeDto)
    {
        var filme = _context.Filmes.FirstOrDefault(filme => filme.Id == id);
        if (filme == null) return NotFound();
        _mapper.Map(filmeDto, filme);
        _context.SaveChanges();
        return NoContent();
    }

    /// <summary>
    /// Atualizar parcialmente as informações de um filme passado pelo ID
    /// </summary>
    /// <param name="id">Objeto com o campo ID necessário para buscar o filme</param>
    /// <param name="patch">op = replace, path = "o que deseja alterar", value = "alteração"</param>
    /// <returns>IActionResult</returns>
    /// <response code="204">Caso a atualização seja executada com sucesso</response>
    /// <response code="404">Caso a busca ou atualização não tenham sucesso</response>
    [HttpPatch("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult AtualizaFilmeParcial(int id, JsonPatchDocument<UpdateFilmeDto> patch)
    {
        var filme = _context.Filmes.FirstOrDefault(filme => filme.Id == id);
        if (filme == null) return NotFound();

        var filmeParaAtualizar = _mapper.Map<UpdateFilmeDto>(filme);

        patch.ApplyTo(filmeParaAtualizar, ModelState);

        if (!TryValidateModel(filmeParaAtualizar))
        {
            return ValidationProblem(ModelState);
        }
        _mapper.Map(filmeParaAtualizar, filme);
        _context.SaveChanges();
        return NoContent();
    }

    /// <summary>
    /// Deletar um filme
    /// </summary>
    /// <param name="id">Objeto com o campo ID necessário para encontrar o filme que deseja deletar</param>
    /// <returns>IActionResult</returns>
    /// <response code="204">Caso a exclusão seja executada com sucesso</response>
    /// <response code="404">Caso a busca ou exclusão não tenham sucesso</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult DeletaFilme(int id)
    {
        var filme = _context.Filmes.FirstOrDefault(filme => filme.Id == id);
        if (filme == null) return NotFound();
        _context.Remove(filme);
        _context.SaveChanges();
        return NoContent();
    }
}
