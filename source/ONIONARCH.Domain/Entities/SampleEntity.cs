using ONIONARCH.Domain.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ONIONARCH.Domain.Entities;

[ExcludeFromCodeCoverage]
public sealed class SampleEntity : Entity
{
    [Key]
    public int SampleId { get; set; }
    [Required]
    public string? SampleString { get; set; }
    public bool SampleBoolean { get; set; }
    public int SampleInt { get; set; }
    public decimal SampleDecimal { get; set; }
}