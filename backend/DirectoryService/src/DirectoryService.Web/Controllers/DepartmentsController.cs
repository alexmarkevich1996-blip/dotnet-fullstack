using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Departments.AddDepartmentLocation;
using DirectoryService.Core.Departments.AddDepartmentPosition;
using DirectoryService.Core.Departments.CreateDepartment;
using DirectoryService.Core.Departments.DeleteDepartment;
using DirectoryService.Core.Departments.GetDepartmentById;
using DirectoryService.Core.Departments.RemoveDepartmentLocation;
using DirectoryService.Core.Departments.RemoveDepartmentPosition;
using DirectoryService.Core.Departments.UpdateDepartment;
using DirectoryService.Domain.Common.Errors;
using DirectoryService.Web.Contracts;
using DirectoryService.Web.Extensions;
using Microsoft.AspNetCore.Mvc;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace DirectoryService.Web.Controllers;

[ApiController]
[Route("[controller]")]
[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Design",
    "S6960:Controllers should not have too many responsibilities",
    Justification = "Each injected handler encapsulates exactly one CQRS use case; the " +
                     "controller stays a thin HTTP boundary that only dispatches to them, it does not " +
                     "itself mix responsibilities.")]
public class DepartmentsController : ControllerBase
{
    private readonly ICommandHandler<CreateDepartmentCommand, Result<Guid, ErrorList>> _createHandler;
    private readonly ICommandHandler<UpdateDepartmentCommand, UnitResult<ErrorList>> _updateHandler;
    private readonly ICommandHandler<AddDepartmentLocationCommand, UnitResult<ErrorList>> _addLocationHandler;
    private readonly ICommandHandler<RemoveDepartmentLocationCommand, UnitResult<ErrorList>> _removeLocationHandler;
    private readonly ICommandHandler<DeleteDepartmentCommand, UnitResult<ErrorList>> _deleteHandler;
    private readonly ICommandHandler<AddDepartmentPositionCommand, UnitResult<ErrorList>> _addPositionHandler;
    private readonly ICommandHandler<RemoveDepartmentPositionCommand, UnitResult<ErrorList>> _removePositionHandler;
    private readonly IQueryHandler<GetDepartmentByIdQuery, Result<DepartmentResponseDto, ErrorList>> _getByIdHandler;

    public DepartmentsController(
        ICommandHandler<CreateDepartmentCommand, Result<Guid, ErrorList>> createHandler,
        ICommandHandler<UpdateDepartmentCommand, UnitResult<ErrorList>> updateHandler,
        ICommandHandler<AddDepartmentLocationCommand, UnitResult<ErrorList>> addLocationHandler,
        ICommandHandler<RemoveDepartmentLocationCommand, UnitResult<ErrorList>> removeLocationHandler,
        ICommandHandler<DeleteDepartmentCommand, UnitResult<ErrorList>> deleteHandler,
        ICommandHandler<AddDepartmentPositionCommand, UnitResult<ErrorList>> addPositionHandler,
        ICommandHandler<RemoveDepartmentPositionCommand, UnitResult<ErrorList>> removePositionHandler,
        IQueryHandler<GetDepartmentByIdQuery, Result<DepartmentResponseDto, ErrorList>> getByIdHandler)
    {
        _createHandler = createHandler;
        _updateHandler = updateHandler;
        _addLocationHandler = addLocationHandler;
        _removeLocationHandler = removeLocationHandler;
        _deleteHandler = deleteHandler;
        _addPositionHandler = addPositionHandler;
        _removePositionHandler = removePositionHandler;
        _getByIdHandler = getByIdHandler;
    }

    [HttpPost]
    [ProducesResponseType(typeof(Envelope<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Envelope<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Envelope<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Envelope<object>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(Envelope<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IResult> Create([FromBody] CreateDepartmentDto request, CancellationToken cancellationToken)
    {
        var command = new CreateDepartmentCommand(request.Name, request.Slug, request.ParentId, request.LocationIds);
        var result = await _createHandler.Handle(command, cancellationToken);
        return result.ToApiResult(StatusCodes.Status201Created);
    }

    [HttpPost("{departmentId:guid}/locations/{locationId:guid}")]
    [ProducesResponseType(typeof(Envelope<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Envelope<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Envelope<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Envelope<object>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(Envelope<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IResult> AddLocation([FromRoute] Guid departmentId, [FromRoute] Guid locationId,
        CancellationToken cancellationToken)
    {
        var command = new AddDepartmentLocationCommand(departmentId, locationId);
        var result = await _addLocationHandler.Handle(command, cancellationToken);
        return result.ToApiResult();
    }

    [HttpDelete("{departmentId:guid}/locations/{locationId:guid}")]
    [ProducesResponseType(typeof(Envelope<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Envelope<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Envelope<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IResult> RemoveLocation([FromRoute] Guid departmentId, [FromRoute] Guid locationId,
        CancellationToken cancellationToken)
    {
        var command = new RemoveDepartmentLocationCommand(departmentId, locationId);
        var result = await _removeLocationHandler.Handle(command, cancellationToken);
        return result.ToApiResult();
    }

    [HttpPatch("{departmentId:guid}")]
    [ProducesResponseType(typeof(Envelope<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Envelope<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Envelope<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Envelope<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IResult> Update([FromRoute] Guid departmentId, [FromBody] UpdateDepartmentDto request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateDepartmentCommand(departmentId, request.Name);
        var result = await _updateHandler.Handle(command, cancellationToken);
        return result.ToApiResult();
    }

    [HttpGet("{departmentId:guid}")]
    [ProducesResponseType(typeof(Envelope<DepartmentResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Envelope<object>), StatusCodes.Status404NotFound)]
    public async Task<IResult> GetById([FromRoute] Guid departmentId, CancellationToken cancellationToken)
    {
        var query = new GetDepartmentByIdQuery(departmentId);
        var result = await _getByIdHandler.Handle(query, cancellationToken);
        return result.ToApiResult();
    }

    [HttpPost("{departmentId:guid}/positions/{positionId:guid}")]
    [ProducesResponseType(typeof(Envelope<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Envelope<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Envelope<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Envelope<object>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(Envelope<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IResult> AddPosition([FromRoute] Guid departmentId, [FromRoute] Guid positionId,
        CancellationToken cancellationToken)
    {
        var command = new AddDepartmentPositionCommand(departmentId, positionId);
        var result = await _addPositionHandler.Handle(command, cancellationToken);
        return result.ToApiResult();
    }

    [HttpDelete("{departmentId:guid}/positions/{positionId:guid}")]
    [ProducesResponseType(typeof(Envelope<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Envelope<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Envelope<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IResult> RemovePosition([FromRoute] Guid departmentId, [FromRoute] Guid positionId,
        CancellationToken cancellationToken)
    {
        var command = new RemoveDepartmentPositionCommand(departmentId, positionId);
        var result = await _removePositionHandler.Handle(command, cancellationToken);
        return result.ToApiResult();
    }

    [HttpGet]
    public IActionResult Get([FromQuery] GetDepartmentDto request, CancellationToken cancellationToken)
    {
        return Ok(Array.Empty<object>());
    }

    [HttpDelete("{departmentId:guid}")]
    [ProducesResponseType(typeof(Envelope<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Envelope<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Envelope<object>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(Envelope<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IResult> Delete([FromRoute] Guid departmentId, CancellationToken cancellationToken)
    {
        var command = new DeleteDepartmentCommand(departmentId);
        var result = await _deleteHandler.Handle(command, cancellationToken);
        return result.ToApiResult();
    }
}