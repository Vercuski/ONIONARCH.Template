using MediatR;
using Microsoft.AspNetCore.Mvc;
using ONIONARCH.Application.Actions.SampleEntity1Dapper.Commands;
using ONIONARCH.Application.Actions.SampleEntity1Dapper.Queries;
using ONIONARCH.Application.Actions.SampleEntity1EFCore.Commands;
using ONIONARCH.Application.Actions.SampleEntity1EFCore.Queries;
using ONIONARCH.Application.Contracts.Dtos;

namespace ONIONARCH.Presentation.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SampleController(IMediator mediator) : ControllerBase
{
    // GET api/<SampleController>/5
    [HttpGet("EFCore/{id}")]
    public async Task<SampleDtoRecord> GetEFCore(int id)
    {
        GetSingleSampleEntity1EFCoreRequest request = new(id);
        var returnValue = await mediator.Send(request, CancellationToken.None);
        return SampleDtoRecord.Create(returnValue);
    }

    // GET api/<SampleController>/5
    [HttpGet("Dapper/{id}")]
    public async Task<SampleDtoRecord?> GetDapper(int id)
    {
        GetSingleSampleEntity1DapperRequest request = new(id);
        var returnValue = await mediator.Send(request, CancellationToken.None);
        return returnValue is not null ? SampleDtoRecord.Create(returnValue) : null;
    }

    // POST api/<SampleController>
    [HttpPost("EFCore")]
    public async Task<int> CreateEFCore([FromBody] CreateSampleRequestDto dto)
    {
        var entity = dto.MapToDomain();
        CreateSampleEntity1EFCoreRequest request = new(entity);
        return await mediator.Send(request, CancellationToken.None);
    }

    // POST api/<SampleController>
    [HttpPost("Dapper")]
    public async Task<int> CreateDapper([FromBody] CreateSampleRequestDto dto)
    {
        var entity = dto.MapToDomain();
        CreateSampleEntity1DapperRequest request = new(entity);
        return await mediator.Send(request, CancellationToken.None);
    }
}
