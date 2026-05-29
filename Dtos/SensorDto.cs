using System.ComponentModel.DataAnnotations;
using AeroSoilAI.Api.Enums;

namespace AeroSoilAI.Api.Dtos;

public class SensorDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O tipo do sensor é obrigatório.")]
    [EnumDataType(typeof(SensorTipo), ErrorMessage = "O tipo do sensor deve ser Umidade ou LDR.")]
    public SensorTipo Tipo { get; set; }

    [Range(0, 999999.99, ErrorMessage = "A última leitura não pode ser negativa.")]
    public decimal UltimaLeitura { get; set; }

    public DateTime DataAtualizacao { get; set; }
}