using MediatR;
using Microsoft.AspNetCore.Mvc;
using ONIONARCH.Application.Actions.SampleEntity1.Queries;
using ONIONARCH.Domain.Entities;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ONIONARCH.Presentation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SampleController(IMediator mediator) : ControllerBase
    {
        // GET api/<SampleController>/5
        [HttpGet("{id}")]
        public async Task<SampleEntityDefinition> Get(int id)
        {
            GetSingleSampleEntity1Request request = new(id);
            var returnValue = await mediator.Send(request, CancellationToken.None);
            return returnValue;
        }
    }
}
