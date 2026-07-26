using MediatR;
using Microsoft.AspNetCore.Mvc;
using ONIONARCH.Application.Actions.SampleEntityDapper.Commands;
using ONIONARCH.Application.Actions.SampleEntityDapper.Queries;
using ONIONARCH.Application.Actions.SampleEntityEFCore.Commands;
using ONIONARCH.Application.Actions.SampleEntityEFCore.Queries;
using ONIONARCH.Application.Contracts.Dtos;

namespace ONIONARCH.Presentation.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SampleController(IMediator mediator) : ControllerBase
{
    // GET api/<SampleController>/5
    [HttpGet("EFCore/{sampleId}")]
    public async Task<SampleDtoRecord> GetEFCore(int sampleId)
    {
        GetSingleSampleEntityEFCoreRequest request = new(sampleId);
        var returnValue = await mediator.Send(request, CancellationToken.None);
        return SampleDtoRecord.Create(returnValue);
    }

    // GET api/<SampleController>/5
    [HttpGet("Dapper/{sampleId}")]
    public async Task<SampleDtoRecord?> GetDapper(int sampleId)
    {
        GetSingleSampleEntityDapperRequest request = new(sampleId);
        var returnValue = await mediator.Send(request, CancellationToken.None);
        return returnValue is not null ? SampleDtoRecord.Create(returnValue) : null;
    }

    // POST api/<SampleController>
    [HttpPost("EFCore")]
    public async Task<int> CreateEFCore([FromBody] CreateSampleRequestDto dto)
    {
        var entity = dto.MapToDomain();
        CreateSampleEntityEFCoreRequest request = new(entity);
        return await mediator.Send(request, CancellationToken.None);
    }

    // POST api/<SampleController>
    [HttpPost("Dapper")]
    public async Task<int> CreateDapper([FromBody] CreateSampleRequestDto dto)
    {
        var entity = dto.MapToDomain();
        CreateSampleEntityDapperRequest request = new(entity);
        return await mediator.Send(request, CancellationToken.None);
    }

    // PUT api/<SampleController>
    [HttpPut("EFCore")]
    public async Task<int> UpdateEFCore([FromBody] UpdateSampleRequestDto dto)
    {
        var entity = dto.MapToDomain();
        UpdateSampleEntityEFCoreRequest request = new(entity);
        return await mediator.Send(request, CancellationToken.None);
    }

    // PUT api/<SampleController>
    [HttpPut("Dapper")]
    public async Task<int> UpdateDapper([FromBody] UpdateSampleRequestDto dto)
    {
        var entity = dto.MapToDomain();
        UpdateSampleEntityDapperRequest request = new(entity);
        return await mediator.Send(request, CancellationToken.None);
    }

    // DELETE api/<SampleController>
    [HttpDelete("EFCore")]
    public async Task<int> DeleteEFCore(int sampleId)
    {
        GetSingleSampleEntityEFCoreRequest request = new(sampleId);
        var entity = await mediator.Send(request, CancellationToken.None);
        DeleteSampleEntityEFCoreRequest deleteRequest = new(entity);
        return await mediator.Send(deleteRequest, CancellationToken.None);
    }

    // DELETE api/<SampleController>
    [HttpDelete("Dapper")]
    public async Task<int> DeleteDapper(int sampleId)
    {
        DeleteSampleEntityDapperRequest request = new(sampleId);
        return await mediator.Send(request, CancellationToken.None);
    }
}
