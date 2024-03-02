using ONIONARCH.Application.Abstractions;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Entities.SampleEntity1Commands.CreateSampleEntity1;

public sealed class CreateSampleEntity1Request(SampleEntity1 sampleEntity) : ICommand<int>
{
    public SampleEntity1 SampleEntity { get; } = sampleEntity;
}
