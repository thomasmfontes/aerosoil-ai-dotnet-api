namespace AeroSoilAI.Api.Dtos;

public class PropriedadeResponseDto
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string Localizacao { get; set; } = string.Empty;

    public decimal Hectares { get; set; }

    public ICollection<SensorDto> Sensores { get; set; } = new List<SensorDto>();
}