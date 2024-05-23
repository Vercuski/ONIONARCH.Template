using MediatR;
using Microsoft.AspNetCore.Mvc;
using ONIONARCH.Application.Entities.SampleEntity1Queries.GetSingleSampleEntity1;
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
        public async Task<SampleEntity1> Get(int id)
        {
            GetSingleSampleEntity1Request request = new GetSingleSampleEntity1Request(id);
            var returnValue = await mediator.Send(request, CancellationToken.None);
            return returnValue;
        }
    }
}
