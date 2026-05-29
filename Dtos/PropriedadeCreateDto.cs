using System.ComponentModel.DataAnnotations;

namespace AeroSoilAI.Api.Dtos;

public class PropriedadeCreateDto
{
    [Required(ErrorMessage = "O nome da propriedade é obrigatório.")]
    [StringLength(120, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 120 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "A localização é obrigatória.")]
    [StringLength(200, ErrorMessage = "A localização deve ter no máximo 200 caracteres.")]
    public string Localizacao { get; set; } = string.Empty;

    [Range(0.01, 999999.99, ErrorMessage = "A quantidade de hectares deve ser maior que zero.")]
    public decimal Hectares { get; set; }

    public ICollection<SensorDto> Sensores { get; set; } = new List<SensorDto>();
}