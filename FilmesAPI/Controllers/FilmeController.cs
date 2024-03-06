using FilmesAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace FilmesAPI.Controllers;

[ApiController]
[Route("[controller]")]

public class FilmeController : ControllerBase
{
    private static List<Filme> filmes = new List<Filme>();
    private static int id = 0;


    [HttpPost]      //Sempre que fizer uma operação post para este controlador, irá executar o que estiver abaixo
    public void AdicionaFilme([FromBody] Filme filme)       //[FromBody] = indica da onde pegará a informação (do corpo)
    {
        filme.Id = id++;
        filmes.Add(filme);
        Console.WriteLine(filme.Titulo);
        Console.WriteLine(filme.Genero);
        Console.WriteLine(filme.Duracao);
    }

    [HttpGet]
    public IEnumerable<Filme> RecuperarFilmes()
    {
        return filmes;
    }

    [HttpGet("{id}")]       // para diferenciar o get, informamos que precisamos receber
    public Filme? RecuperaFilmePorId(int id)
    {
        return filmes.FirstOrDefault(filme => filme.Id == id);
    }
}
