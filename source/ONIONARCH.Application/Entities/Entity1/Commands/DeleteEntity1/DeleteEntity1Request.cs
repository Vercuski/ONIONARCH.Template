using ONIONARCH.Application.Abstractions;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Entities.Entity1.Commands.DeleteEntity1;

public sealed class DeleteEntity1Request(SampleEntity sampleEntity) : ICommand<int>
{
    public SampleEntity SampleEntity { get; } = sampleEntity;
}
