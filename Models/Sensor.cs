using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AeroSoilAI.Api.Enums;

namespace AeroSoilAI.Api.Models;

[Table("TB_SENSOR")]
public class Sensor
{
    [Key]
    [Column("ID_SENSOR")]
    public int Id { get; set; }

    [Required(ErrorMessage = "O tipo do sensor é obrigatório.")]
    [EnumDataType(typeof(SensorTipo), ErrorMessage = "O tipo do sensor deve ser Umidade ou LDR.")]
    [Column("TP_SENSOR")]
    public SensorTipo Tipo { get; set; }

    [Range(0, 999999.99, ErrorMessage = "A última leitura não pode ser negativa.")]
    [Column("VL_ULTIMA_LEITURA", TypeName = "NUMBER(10,2)")]
    public decimal UltimaLeitura { get; set; }

    [Required]
    [Column("DT_ATUALIZACAO")]
    public DateTime DataAtualizacao { get; set; } = DateTime.UtcNow;

    [Required]
    [ForeignKey(nameof(Propriedade))]
    [Column("ID_PROPRIEDADE")]
    public int PropriedadeId { get; set; }

    public Propriedade? Propriedade { get; set; }
}