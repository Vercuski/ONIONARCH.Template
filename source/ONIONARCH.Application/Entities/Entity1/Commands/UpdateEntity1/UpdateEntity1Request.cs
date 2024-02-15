using ONIONARCH.Application.Abstractions;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Entities.Entity1.Commands.UpdateEntity1;

public sealed class UpdateEntity1Request(SampleEntity sampleEntity) : ICommand<int>
{
    public SampleEntity SampleEntity { get; } = sampleEntity;
}
