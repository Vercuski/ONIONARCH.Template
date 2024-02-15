using ONIONARCH.Application.Abstractions;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Entities.Entity1.Commands.CreateEntity1;

public sealed class CreateEntity1Request(SampleEntity sampleEntity) : ICommand<int>
{
    public SampleEntity SampleEntity { get; } = sampleEntity;
}
