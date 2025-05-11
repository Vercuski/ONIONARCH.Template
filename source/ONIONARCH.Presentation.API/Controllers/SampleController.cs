using MediatR;
using Microsoft.AspNetCore.Mvc;
using ONIONARCH.Application.Actions.SampleEntity1Dapper.Queries;
using ONIONARCH.Application.Actions.SampleEntity1EFCore.Queries;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Presentation.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SampleController(IMediator mediator) : ControllerBase
{
    // GET api/<SampleController>/5
    [HttpGet("EFCore/{id}")]
    public async Task<SampleEntityDefinition> GetEFCore(int id)
    {
        GetSingleSampleEntity1EFCoreRequest request = new(id);
        var returnValue = await mediator.Send(request, CancellationToken.None);
        return returnValue;
    }

    // GET api/<SampleController>/5
    [HttpGet("Dapper/{id}")]
    public async Task<SampleEntityDefinition> GetDapper(int id)
    {
        GetSingleSampleEntity1DapperRequest request = new(id);
        var returnValue = await mediator.Send(request, CancellationToken.None);
        return returnValue;
    }
}
