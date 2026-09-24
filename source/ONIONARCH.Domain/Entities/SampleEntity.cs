using ONIONARCH.Domain.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ONIONARCH.Domain.Entities;

/// <summary>
/// Sample domain entity used to demonstrate both the EF Core and Dapper persistence paths
/// end to end. Replace or extend it with real entities when building on this template.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class SampleEntityDefinition : Entity
{
    /// <summary>
    /// Gets or sets the primary key of the sample entity.
    /// </summary>
    [Key]
    public int SampleId { get; set; }

    /// <summary>
    /// Gets or sets a required sample string value.
    /// </summary>
    [Required]
    public string? SampleString { get; set; }

    /// <summary>
    /// Gets or sets a sample Boolean value.
    /// </summary>
    public bool SampleBoolean { get; set; }

    /// <summary>
    /// Gets or sets a sample integer value.
    /// </summary>
    public int SampleInt { get; set; }

    /// <summary>
    /// Gets or sets a sample decimal value.
    /// </summary>
    public decimal SampleDecimal { get; set; }
}