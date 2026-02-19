using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConfigAdmin.Api.Entities;
using ConfigAdmin.Api.Data;

namespace ConfigAdmin.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConfigurationsController : ControllerBase
{
    private readonly ConfigDbContext _db;

    public ConfigurationsController(ConfigDbContext db)
    {
        _db = db;
    }

    // Listeleme + Filtreleme
    [HttpGet]
    public async Task<ActionResult<List<ConfigRecord>>> GetAll(
        [FromQuery] string? name,
        [FromQuery] string? applicationName)
    {
        var query = _db.Configurations.AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(x => x.Name.Contains(name));

        if (!string.IsNullOrWhiteSpace(applicationName))
            query = query.Where(x => x.ApplicationName == applicationName);

        var result = await query
            .OrderBy(x => x.ApplicationName)
            .ThenBy(x => x.Name)
            .ToListAsync();

        return Ok(result);
    }

    // Tek kayıt getir
    [HttpGet("{id}")]
    public async Task<ActionResult<ConfigRecord>> GetById(int id)
    {
        var record = await _db.Configurations.FindAsync(id);

        if (record == null)
            return NotFound();

        return Ok(record);
    }

    // Ekleme
    [HttpPost]
    public async Task<ActionResult<ConfigRecord>> Add(ConfigRecord record)
    {
        record.ModifiedAt = DateTime.UtcNow;

        _db.Configurations.Add(record);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = record.Id }, record);
    }

    // Güncelleme
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, ConfigRecord input)
    {
        var record = await _db.Configurations.FindAsync(id);

        if (record == null)
            return NotFound();

        record.Value = input.Value;
        record.Type = input.Type;
        record.IsActive = input.IsActive;
        record.ApplicationName = input.ApplicationName;
        record.ModifiedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return NoContent();
    }

    // Silme (opsiyonel ama düzgün API'de olmalı)
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var record = await _db.Configurations.FindAsync(id);

        if (record == null)
            return NotFound();

        _db.Configurations.Remove(record);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
