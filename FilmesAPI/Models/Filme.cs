using System.ComponentModel.DataAnnotations;

namespace FilmesAPI.Models;

public class Filme
{
    [Key]
    [Required]
    public int Id { get; set; }
    [Required(ErrorMessage = "O título do filme é obrigatório.")]      // Required impõe que o campo é obrigatório
    public string Titulo { get; set; }
    [Required(ErrorMessage = "O genero do filme é obrigatório.")]
    [MaxLength(50, ErrorMessage = "O tamanho do gênero não pode exceder 50 caractéres.")]   // Max Length define o máximo de caractéres
    public string Genero { get; set; }
    [Required(ErrorMessage = "A duração do filme é obrigatório.")]
    [Range(70, 600, ErrorMessage = "A duração deve ter entre 70 a 600 minutos.")]       // Range define um mínimo e máximo
    public int Duracao { get; set; }
    public virtual ICollection<Sessao> Sessoes { get; set; }
}
