using CSharpFunctionalExtensions;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Positions;
using DirectoryService.Domain.Common.Errors;
using DirectoryService.Domain.Departments;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Core.Departments.AddDepartmentPosition;

public partial class AddDepartmentPositionCommandHandler
    : ICommandHandler<AddDepartmentPositionCommand, UnitResult<ErrorList>>
{
    private readonly IDepartmentsRepository _departmentsRepository;
    private readonly IPositionsRepository _positionsRepository;
    private readonly ITransactionManager _transactionManager;
    private readonly ILogger<AddDepartmentPositionCommandHandler> _logger;

    public AddDepartmentPositionCommandHandler(
        IDepartmentsRepository departmentsRepository,
        IPositionsRepository positionsRepository,
        ITransactionManager transactionManager,
        ILogger<AddDepartmentPositionCommandHandler> logger)
    {
        _departmentsRepository = departmentsRepository;
        _positionsRepository = positionsRepository;
        _transactionManager = transactionManager;
        _logger = logger;
    }

    public async Task<UnitResult<ErrorList>> Handle(
        AddDepartmentPositionCommand command, CancellationToken cancellationToken)
    {
        var positionResult = await _positionsRepository.GetByIdAsync(command.PositionId, cancellationToken);
        if (positionResult.IsFailure)
            return positionResult.Error;

        var departmentResult = await _departmentsRepository.GetByIdAsync(command.DepartmentId, cancellationToken);
        if (departmentResult.IsFailure)
            return departmentResult.Error;

        var linkExists = await _departmentsRepository.PositionLinkExistsAsync(
            command.DepartmentId, command.PositionId, cancellationToken);
        if (linkExists)
        {
            LogDepartmentPositionConflict(command.DepartmentId, command.PositionId);
            return Error.Conflict("department_position.already_exists",
                $"Position '{command.PositionId}' is already linked to department '{command.DepartmentId}'").ToErrors();
        }

        var departmentPositionResult = DepartmentPosition.Create(command.DepartmentId, command.PositionId);
        if (departmentPositionResult.IsFailure)
            return departmentPositionResult.Error;

        var stageResult = await _departmentsRepository.AddPositionAsync(departmentPositionResult.Value, cancellationToken);
        if (stageResult.IsFailure)
            return stageResult.Error;

        var result = await _transactionManager.SaveChangesAsync(cancellationToken);
        if (result.IsSuccess)
            LogPositionAddedToDepartment(command.DepartmentId, command.PositionId);

        return result;
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Position {PositionId} added to department {DepartmentId}")]
    private partial void LogPositionAddedToDepartment(Guid departmentId, Guid positionId);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Attempted to link already-linked position {PositionId} to department {DepartmentId}")]
    private partial void LogDepartmentPositionConflict(Guid departmentId, Guid positionId);
}
