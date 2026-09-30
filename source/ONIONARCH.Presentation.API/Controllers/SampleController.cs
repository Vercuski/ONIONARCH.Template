using Microsoft.AspNetCore.Mvc;
using ONIONARCH.Application.Abstractions;
//#if (HasDapper)
using ONIONARCH.Application.Actions.SampleEntityDapper.Commands;
using ONIONARCH.Application.Actions.SampleEntityDapper.Queries;
//#endif
//#if (HasEfCore)
using ONIONARCH.Application.Actions.SampleEntityEFCore.Commands;
using ONIONARCH.Application.Actions.SampleEntityEFCore.Queries;
//#endif
using ONIONARCH.Application.Contracts.Dtos;
using ONIONARCH.Presentation.API.Extensions;

namespace ONIONARCH.Presentation.API.Controllers;

/// <summary>
/// Sample CRUD endpoints exposing the same operations over both persistence paths:
/// <c>api/Sample/EFCore/...</c> and <c>api/Sample/Dapper/...</c>.
/// </summary>
/// <remarks>
/// The controller depends only on <see cref="ISender"/> and Application contract types; it never
/// references Persistence (enforced by the Presentation architecture tests).
/// </remarks>
/// <param name="sender">Dispatches requests to their Application-layer handlers.</param>
[Route("api/[controller]")]
[ApiController]
public class SampleController(ISender sender) : ControllerBase
{
//#if (HasEfCore)
    /// <summary>
    /// Gets a sample entity by key via EF Core.
    /// </summary>
    /// <remarks><c>GET api/Sample/EFCore/{sampleId}</c></remarks>
    /// <param name="sampleId">The key of the entity to retrieve.</param>
    /// <returns>200 with a <see cref="SampleDtoRecord"/>, or 404 if not found.</returns>
    [HttpGet("EFCore/{sampleId}")]
    public async Task<IActionResult> GetEFCore(int sampleId)
    {
        GetSingleSampleEntityEFCoreRequest request = new(sampleId);
        var result = await sender.Send(request, CancellationToken.None);
        return result.ToActionResult(this, SampleDtoRecord.Create);
    }
//#endif
//#if (HasDapper)
//#if (HasEfCore)

//#endif
    /// <summary>
    /// Gets a sample entity by key via Dapper.
    /// </summary>
    /// <remarks><c>GET api/Sample/Dapper/{sampleId}</c></remarks>
    /// <param name="sampleId">The key of the entity to retrieve.</param>
    /// <returns>200 with a <see cref="SampleDtoRecord"/>, or 404 if not found.</returns>
    [HttpGet("Dapper/{sampleId}")]
    public async Task<IActionResult> GetDapper(int sampleId)
    {
        GetSingleSampleEntityDapperRequest request = new(sampleId);
        var result = await sender.Send(request, CancellationToken.None);
        return result.ToActionResult(this, SampleDtoRecord.Create);
    }
//#endif
//#if (HasEfCore)

    /// <summary>
    /// Creates a sample entity via EF Core.
    /// </summary>
    /// <remarks><c>POST api/Sample/EFCore</c></remarks>
    /// <param name="dto">The values for the new entity.</param>
    /// <returns>200 with the number of state entries written.</returns>
    [HttpPost("EFCore")]
    public async Task<IActionResult> CreateEFCore([FromBody] CreateSampleRequestDto dto)
    {
        var entity = dto.MapToDomain();
        CreateSampleEntityEFCoreRequest request = new(entity);
        var result = await sender.Send(request, CancellationToken.None);
        return result.ToActionResult(this);
    }
//#endif
//#if (HasDapper)

    /// <summary>
    /// Creates a sample entity via Dapper.
    /// </summary>
    /// <remarks><c>POST api/Sample/Dapper</c></remarks>
    /// <param name="dto">The values for the new entity.</param>
    /// <returns>200 with the number of rows affected.</returns>
    [HttpPost("Dapper")]
    public async Task<IActionResult> CreateDapper([FromBody] CreateSampleRequestDto dto)
    {
        var entity = dto.MapToDomain();
        CreateSampleEntityDapperRequest request = new(entity);
        var result = await sender.Send(request, CancellationToken.None);
        return result.ToActionResult(this);
    }
//#endif
//#if (HasEfCore)

    /// <summary>
    /// Updates a sample entity via EF Core.
    /// </summary>
    /// <remarks><c>PUT api/Sample/EFCore</c></remarks>
    /// <param name="dto">The key of the entity to update and its new values.</param>
    /// <returns>200 with the number of state entries written.</returns>
    [HttpPut("EFCore")]
    public async Task<IActionResult> UpdateEFCore([FromBody] UpdateSampleRequestDto dto)
    {
        var entity = dto.MapToDomain();
        UpdateSampleEntityEFCoreRequest request = new(entity);
        var result = await sender.Send(request, CancellationToken.None);
        return result.ToActionResult(this);
    }
//#endif
//#if (HasDapper)

    /// <summary>
    /// Updates a sample entity via Dapper.
    /// </summary>
    /// <remarks><c>PUT api/Sample/Dapper</c></remarks>
    /// <param name="dto">The key of the entity to update and its new values.</param>
    /// <returns>200 with the number of rows affected (0 if no row matched).</returns>
    [HttpPut("Dapper")]
    public async Task<IActionResult> UpdateDapper([FromBody] UpdateSampleRequestDto dto)
    {
        var entity = dto.MapToDomain();
        UpdateSampleEntityDapperRequest request = new(entity);
        var result = await sender.Send(request, CancellationToken.None);
        return result.ToActionResult(this);
    }
//#endif
//#if (HasEfCore)

    /// <summary>
    /// Deletes a sample entity via EF Core. The entity is loaded first so a missing key is
    /// reported as 404 rather than as a failed delete.
    /// </summary>
    /// <remarks><c>DELETE api/Sample/EFCore?sampleId={sampleId}</c></remarks>
    /// <param name="sampleId">The key of the entity to delete (bound from the query string).</param>
    /// <returns>200 with the number of state entries written, or the mapped error response if the lookup failed.</returns>
    [HttpDelete("EFCore")]
    public async Task<IActionResult> DeleteEFCore(int sampleId)
    {
        GetSingleSampleEntityEFCoreRequest request = new(sampleId);
        var entity = await sender.Send(request, CancellationToken.None);
        if (!entity.IsSuccess || entity.Value is null)
        {
            return entity.ErrorType switch
            {
                ResultErrorType.NotFound => NotFound(entity.Error),
                ResultErrorType.Validation => BadRequest(entity.Error),
                ResultErrorType.Conflict => Conflict(entity.Error),
                _ => Problem(entity.Error)
            };
        }
        else
        {
            DeleteSampleEntityEFCoreRequest deleteRequest = new(entity.Value);
            var result = await sender.Send(deleteRequest, CancellationToken.None);
            return result.ToActionResult(this);
        }
    }
//#endif
//#if (HasDapper)

    /// <summary>
    /// Deletes a sample entity via Dapper.
    /// </summary>
    /// <remarks><c>DELETE api/Sample/Dapper?sampleId={sampleId}</c></remarks>
    /// <param name="sampleId">The key of the entity to delete (bound from the query string).</param>
    /// <returns>200 with the number of rows affected (0 if no row matched).</returns>
    [HttpDelete("Dapper")]
    public async Task<IActionResult> DeleteDapper(int sampleId)
    {
        DeleteSampleEntityDapperRequest request = new(sampleId);
        var result = await sender.Send(request, CancellationToken.None);
        return result.ToActionResult(this);
    }
//#endif
//#if (HasEfCore)

    /// <summary>
    /// Creates many sample entities in one bulk insert via the EF Core bulk port.
    /// </summary>
    /// <remarks><c>POST api/Sample/EFCore/Bulk</c>. Not available when the command database is MySQL (see README).</remarks>
    /// <param name="dtos">The values for the new entities.</param>
    /// <returns>200 with the generated keys in request order, or 400 if the list is empty.</returns>
    [HttpPost("EFCore/Bulk")]
    public async Task<IActionResult> BulkCreateEFCore([FromBody] IReadOnlyList<CreateSampleRequestDto> dtos)
    {
        var entities = dtos.Select(dto => dto.MapToDomain()).ToList();
        BulkCreateSampleEntityEFCoreRequest request = new(entities);
        var result = await sender.Send(request, CancellationToken.None);
        return result.ToActionResult(this);
    }

    /// <summary>
    /// Inserts or updates many sample entities by key in one bulk operation via the EF Core bulk port.
    /// An item with <c>dtoSampleId</c> 0 is inserted; any other key updates the matching row.
    /// </summary>
    /// <remarks><c>PUT api/Sample/EFCore/Bulk</c>. Not available when the command database is MySQL (see README).</remarks>
    /// <param name="dtos">The keys and values to write.</param>
    /// <returns>200 with the number of entities written, or 400 if the list is empty.</returns>
    [HttpPut("EFCore/Bulk")]
    public async Task<IActionResult> BulkUpsertEFCore([FromBody] IReadOnlyList<UpdateSampleRequestDto> dtos)
    {
        var entities = dtos.Select(dto => dto.MapToDomain()).ToList();
        BulkUpsertSampleEntityEFCoreRequest request = new(entities);
        var result = await sender.Send(request, CancellationToken.None);
        return result.ToActionResult(this);
    }

    /// <summary>
    /// Updates many sample entities with one set-based statement via the EF Core bulk port.
    /// </summary>
    /// <remarks><c>PATCH api/Sample/EFCore/Bulk</c></remarks>
    /// <param name="dto">The keys to update and the changes to apply.</param>
    /// <returns>200 with the number of rows updated, or 400 if no keys were supplied.</returns>
    [HttpPatch("EFCore/Bulk")]
    public async Task<IActionResult> BulkUpdateEFCore([FromBody] BulkUpdateSampleRequestDto dto)
    {
        BulkUpdateSampleEntityEFCoreRequest request = new(dto.DtoSampleIds, dto.DtoSampleBoolean, dto.DtoSampleIntIncrement);
        var result = await sender.Send(request, CancellationToken.None);
        return result.ToActionResult(this);
    }

    /// <summary>
    /// Deletes many sample entities with one set-based statement via the EF Core bulk port.
    /// </summary>
    /// <remarks><c>DELETE api/Sample/EFCore/Bulk?sampleIds=1&amp;sampleIds=2</c></remarks>
    /// <param name="sampleIds">The keys of the entities to delete (bound from the query string).</param>
    /// <returns>200 with the number of rows deleted, or 400 if no keys were supplied.</returns>
    [HttpDelete("EFCore/Bulk")]
    public async Task<IActionResult> BulkDeleteEFCore([FromQuery] int[] sampleIds)
    {
        BulkDeleteSampleEntityEFCoreRequest request = new(sampleIds);
        var result = await sender.Send(request, CancellationToken.None);
        return result.ToActionResult(this);
    }
//#endif
}
