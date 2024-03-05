using FilmesAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace FilmesAPI.Controllers;

[ApiController]
[Route("[controller]")]

public class FilmeController : ControllerBase
{
    private static List<Filme> filmes = new List<Filme>();

    [HttpPost]      //Sempre que fizer uma operação post para este controlador, irá executar o que estiver abaixo
    public void AdicionaFilme([FromBody] Filme filme)
    {
        filmes.Add(filme);
    }
}
