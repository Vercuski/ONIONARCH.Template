using ONIONARCH.Application.Abstractions;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Entities.SampleEntity1Commands.DeleteSampleEntity1;

public sealed class DeleteSampleEntity1Request(SampleEntity1 sampleEntity) : ICommand<int>
{
    public SampleEntity1 SampleEntity { get; } = sampleEntity;
}
