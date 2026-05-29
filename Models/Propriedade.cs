using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AeroSoilAI.Api.Models;

[Table("TB_PROPRIEDADE")]
public class Propriedade
{
    [Key]
    [Column("ID_PROPRIEDADE")]
    public int Id { get; set; }

    [Required(ErrorMessage = "O nome da propriedade é obrigatório.")]
    [StringLength(120, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 120 caracteres.")]
    [Column("NM_PROPRIEDADE")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "A localização é obrigatória.")]
    [StringLength(200, ErrorMessage = "A localização deve ter no máximo 200 caracteres.")]
    [Column("DS_LOCALIZACAO")]
    public string Localizacao { get; set; } = string.Empty;

    [Range(0.01, 999999.99, ErrorMessage = "A quantidade de hectares deve ser maior que zero.")]
    [Column("NR_HECTARES", TypeName = "NUMBER(10,2)")]
    public decimal Hectares { get; set; }

    public ICollection<Sensor> Sensores { get; set; } = new List<Sensor>();
}