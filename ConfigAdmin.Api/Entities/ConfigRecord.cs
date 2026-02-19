using System.ComponentModel.DataAnnotations;

namespace ConfigAdmin.Api.Entities;

public class ConfigRecord
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; }

    [Required]
    [MaxLength(50)]
    public string Type { get; set; }

    [Required]
    [MaxLength(500)]
    public string Value { get; set; }

    public bool IsActive { get; set; }

    [Required]
    [MaxLength(100)]
    public string ApplicationName { get; set; }

    public DateTime ModifiedAt { get; set; }
}
