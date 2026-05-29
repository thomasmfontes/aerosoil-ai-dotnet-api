using AeroSoilAI.Api.Data;
using AeroSoilAI.Api.Dtos;
using AeroSoilAI.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

    [HttpGet]
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

    [HttpGet("{id:int}")]
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

    [HttpPost]
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

    [HttpPut("{id:int}")]
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

    [HttpDelete("{id:int}")]
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