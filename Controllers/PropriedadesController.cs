using AeroSoilAI.Api.Data;
using AeroSoilAI.Api.Dtos;
using AeroSoilAI.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace AeroSoilAI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PropriedadesController : ControllerBase
{
    private readonly AppDbContext _context;

    public PropriedadesController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Recupera todas as propriedades registradas.
    /// </summary>
    /// <remarks>
    /// Retorna uma lista completa com todas as propriedades rurais cadastradas no sistema,
    /// incluindo os sensores associados a cada uma delas.
    /// </remarks>
    /// <response code="200">Retorna a lista de propriedades encontradas.</response>
    /// <response code="404">Caso nenhuma propriedade esteja cadastrada no banco de dados.</response>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Obtém todas as propriedades",
        Description = "Retorna uma lista com todas as propriedades cadastradas no sistema, incluindo seus respectivos sensores."
    )]
    [ProducesResponseType(typeof(IEnumerable<PropriedadeResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<PropriedadeResponseDto>>> GetAll()
    {
        var propriedades = await _context.Propriedades
            .AsNoTracking()
            .Include(p => p.Sensores)
            .OrderBy(p => p.Id)
            .Select(p => new PropriedadeResponseDto
            {
                Id = p.Id,
                Nome = p.Nome,
                Localizacao = p.Localizacao,
                Hectares = p.Hectares,
                Sensores = p.Sensores.Select(s => new SensorDto
                {
                    Id = s.Id,
                    Tipo = s.Tipo,
                    UltimaLeitura = s.UltimaLeitura,
                    DataAtualizacao = s.DataAtualizacao
                }).ToList()
            })
            .ToListAsync();

        if (!propriedades.Any())
        {
            return NotFound(new
            {
                mensagem = "Nenhuma propriedade cadastrada no momento."
            });
        }

        return Ok(propriedades);
    }

    /// <summary>
    /// Recupera uma propriedade específica pelo ID.
    /// </summary>
    /// <remarks>
    /// Busca e retorna os detalhes de uma propriedade cadastrada com base em seu identificador único.
    /// </remarks>
    /// <param name="id">Identificador único da propriedade.</param>
    /// <response code="200">Retorna os detalhes da propriedade solicitada.</response>
    /// <response code="404">Caso não exista nenhuma propriedade com o ID informado.</response>
    [HttpGet("{id:int}")]
    [SwaggerOperation(
        Summary = "Obtém uma propriedade por ID",
        Description = "Busca os detalhes de uma propriedade cadastrada com base em seu identificador único."
    )]
    [ProducesResponseType(typeof(PropriedadeResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PropriedadeResponseDto>> GetById(int id)
    {
        var propriedade = await _context.Propriedades
            .AsNoTracking()
            .Include(p => p.Sensores)
            .Where(p => p.Id == id)
            .Select(p => new PropriedadeResponseDto
            {
                Id = p.Id,
                Nome = p.Nome,
                Localizacao = p.Localizacao,
                Hectares = p.Hectares,
                Sensores = p.Sensores.Select(s => new SensorDto
                {
                    Id = s.Id,
                    Tipo = s.Tipo,
                    UltimaLeitura = s.UltimaLeitura,
                    DataAtualizacao = s.DataAtualizacao
                }).ToList()
            })
            .FirstOrDefaultAsync();

        if (propriedade is null)
        {
            return NotFound(new
            {
                mensagem = $"Nenhuma propriedade encontrada com o ID {id}."
            });
        }

        return Ok(propriedade);
    }

    /// <summary>
    /// Cadastra uma nova propriedade.
    /// </summary>
    /// <remarks>
    /// Cria um novo registro de propriedade no sistema juntamente com os sensores fornecidos no payload.
    /// 
    /// Exemplo de payload:
    /// 
    ///     POST /api/Propriedades
    ///     {
    ///        "nome": "Fazenda Sol Nascente",
    ///        "localizacao": "Ribeirão Preto - SP",
    ///        "hectares": 120.5,
    ///        "sensores": [
    ///           {
    ///              "tipo": "Umidade",
    ///              "ultimaLeitura": 45.2,
    ///              "dataAtualizacao": "2026-06-09T19:24:00Z"
    ///           }
    ///        ]
    ///     }
    /// 
    /// </remarks>
    /// <param name="dto">Dados de criação da propriedade.</param>
    /// <response code="201">Propriedade criada com sucesso.</response>
    /// <response code="400">Dados inválidos fornecidos no payload.</response>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Cadastra uma nova propriedade",
        Description = "Cria um novo registro de propriedade no sistema juntamente com os sensores fornecidos no payload."
    )]
    [ProducesResponseType(typeof(PropriedadeResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PropriedadeResponseDto>> Create([FromBody] PropriedadeCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var propriedade = new Propriedade
        {
            Nome = dto.Nome.Trim(),
            Localizacao = dto.Localizacao.Trim(),
            Hectares = dto.Hectares,
            Sensores = dto.Sensores.Select(sensorDto => new Sensor
            {
                Tipo = sensorDto.Tipo,
                UltimaLeitura = sensorDto.UltimaLeitura,
                DataAtualizacao = sensorDto.DataAtualizacao == default
                    ? DateTime.UtcNow
                    : sensorDto.DataAtualizacao
            }).ToList()
        };

        _context.Propriedades.Add(propriedade);
        await _context.SaveChangesAsync();

        var response = new PropriedadeResponseDto
        {
            Id = propriedade.Id,
            Nome = propriedade.Nome,
            Localizacao = propriedade.Localizacao,
            Hectares = propriedade.Hectares,
            Sensores = propriedade.Sensores.Select(s => new SensorDto
            {
                Id = s.Id,
                Tipo = s.Tipo,
                UltimaLeitura = s.UltimaLeitura,
                DataAtualizacao = s.DataAtualizacao
            }).ToList()
        };

        return CreatedAtAction(nameof(GetById), new { id = propriedade.Id }, response);
    }

    /// <summary>
    /// Atualiza uma propriedade existente.
    /// </summary>
    /// <remarks>
    /// Atualiza as informações básicas (nome, localização, hectares) de uma propriedade específica identificada pelo ID da rota.
    /// Não altera sensores diretamente por meio deste endpoint.
    /// </remarks>
    /// <param name="id">Identificador da propriedade a ser atualizada.</param>
    /// <param name="dto">Dados atualizados da propriedade.</param>
    /// <response code="200">Propriedade atualizada com sucesso.</response>
    /// <response code="400">ID da rota inválido (menor ou igual a zero) ou dados do payload inválidos.</response>
    /// <response code="404">Propriedade não encontrada.</response>
    [HttpPut("{id:int}")]
    [SwaggerOperation(
        Summary = "Atualiza uma propriedade existente",
        Description = "Atualiza as informações básicas (nome, localização, hectares) de uma propriedade específica. Não altera sensores diretamente."
    )]
    [ProducesResponseType(typeof(PropriedadeResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PropriedadeResponseDto>> Update(int id, [FromBody] PropriedadeUpdateDto dto)
    {
        if (id <= 0)
        {
            return BadRequest(new
            {
                mensagem = "O ID informado na rota deve ser maior que zero."
            });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var propriedade = await _context.Propriedades
            .Include(p => p.Sensores)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (propriedade is null)
        {
            return NotFound(new
            {
                mensagem = $"Nenhuma propriedade encontrada com o ID {id}."
            });
        }

        propriedade.Nome = dto.Nome.Trim();
        propriedade.Localizacao = dto.Localizacao.Trim();
        propriedade.Hectares = dto.Hectares;

        await _context.SaveChangesAsync();

        var response = new PropriedadeResponseDto
        {
            Id = propriedade.Id,
            Nome = propriedade.Nome,
            Localizacao = propriedade.Localizacao,
            Hectares = propriedade.Hectares,
            Sensores = propriedade.Sensores.Select(s => new SensorDto
            {
                Id = s.Id,
                Tipo = s.Tipo,
                UltimaLeitura = s.UltimaLeitura,
                DataAtualizacao = s.DataAtualizacao
            }).ToList()
        };

        return Ok(response);
    }

    /// <summary>
    /// Remove uma propriedade pelo ID.
    /// </summary>
    /// <remarks>
    /// Exclui definitivamente uma propriedade e todos os sensores vinculados a ela do banco de dados.
    /// </remarks>
    /// <param name="id">Identificador único da propriedade a ser removida.</param>
    /// <response code="204">Propriedade removida com sucesso.</response>
    /// <response code="404">Propriedade não encontrada.</response>
    [HttpDelete("{id:int}")]
    [SwaggerOperation(
        Summary = "Remove uma propriedade pelo ID",
        Description = "Exclui definitivamente uma propriedade e seus sensores vinculados do banco de dados."
    )]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(int id)
    {
        var propriedade = await _context.Propriedades
            .FirstOrDefaultAsync(p => p.Id == id);

        if (propriedade is null)
        {
            return NotFound(new
            {
                mensagem = $"Nenhuma propriedade encontrada com o ID {id}."
            });
        }

        _context.Propriedades.Remove(propriedade);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}