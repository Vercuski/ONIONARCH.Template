using ONIONARCH.Domain.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ONIONARCH.Domain.Entities;

[ExcludeFromCodeCoverage]
public sealed class SampleEntity2 : Entity
{
    [Key]
    public int SampleId2 { get; set; }
    [Required]
    public string? SampleString2 { get; set; }
    public bool SampleBoolean2 { get; set; }
    public int SampleInt2 { get; set; }
    public decimal SampleDecimal2 { get; set; }
}