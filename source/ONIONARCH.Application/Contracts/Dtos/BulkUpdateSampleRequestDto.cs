using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Contracts.Dtos;

/// <summary>
/// Inbound payload for a set-based update of many sample entities.
/// </summary>
/// <param name="DtoSampleIds">The keys of the entities to update.</param>
/// <param name="DtoSampleBoolean">The value to assign to <see cref="SampleEntityDefinition.SampleBoolean"/>.</param>
/// <param name="DtoSampleIntIncrement">The amount to add to each entity's current <see cref="SampleEntityDefinition.SampleInt"/>.</param>
public sealed record BulkUpdateSampleRequestDto(
    IReadOnlyList<int> DtoSampleIds,
    bool DtoSampleBoolean,
    int DtoSampleIntIncrement);
