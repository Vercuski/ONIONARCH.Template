using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Contracts.Dtos;

/// <summary>
/// Outbound representation of a sample entity returned to API callers.
/// </summary>
/// <param name="Id">The entity's key, taken from <see cref="SampleEntityDefinition.SampleId"/>.</param>
public sealed record SampleDtoRecord(int Id)
{
    /// <summary>
    /// Gets the sample identifier.
    /// </summary>
    /// <remarks>
    /// <see cref="Create"/> does not currently assign this property, so it serializes as <c>0</c>;
    /// the entity's key is carried by <see cref="Id"/>.
    /// </remarks>
    public int DtoSampleId { get; private set; }

    /// <summary>
    /// Gets the sample string value.
    /// </summary>
    public string? DtoSampleString { get; private set; }

    /// <summary>
    /// Gets the sample Boolean value.
    /// </summary>
    public bool DtoSampleBoolean { get; private set; }

    /// <summary>
    /// Gets the sample integer value.
    /// </summary>
    public int DtoSampleInt { get; private set; }

    /// <summary>
    /// Gets the sample decimal value.
    /// </summary>
    public decimal DtoSampleDecimal { get; private set; }

    /// <summary>
    /// Creates a DTO from a domain entity.
    /// </summary>
    /// <param name="entity">The entity to project.</param>
    /// <returns>A new <see cref="SampleDtoRecord"/> populated from <paramref name="entity"/>.</returns>
    public static SampleDtoRecord Create(SampleEntityDefinition entity)
    {
        return new SampleDtoRecord(entity.SampleId)
        {
            DtoSampleString = entity.SampleString,
            DtoSampleBoolean = entity.SampleBoolean,
            DtoSampleInt = entity.SampleInt,
            DtoSampleDecimal = entity.SampleDecimal,
        };
    }
}
