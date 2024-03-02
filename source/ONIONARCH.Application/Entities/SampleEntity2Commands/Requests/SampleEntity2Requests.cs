using ONIONARCH.Application.Abstractions;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Entities.SampleEntity2Commands.Requests;

public sealed class CreateSampleEntity2Request(SampleEntity2 sampleEntity) : ICommand<int>
{
    public SampleEntity2 SampleEntity { get; } = sampleEntity;
}

public sealed class DeleteSampleEntity2Request(SampleEntity2 sampleEntity) : ICommand<int>
{
    public SampleEntity2 SampleEntity { get; } = sampleEntity;
}

public sealed class UpdateSampleEntity2Request(SampleEntity2 sampleEntity) : ICommand<int>
{
    public SampleEntity2 SampleEntity { get; } = sampleEntity;
}