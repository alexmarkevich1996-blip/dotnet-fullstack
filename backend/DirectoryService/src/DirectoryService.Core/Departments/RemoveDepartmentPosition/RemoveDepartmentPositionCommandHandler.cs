using CSharpFunctionalExtensions;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Positions;
using DirectoryService.Domain.Common.Errors;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Core.Departments.RemoveDepartmentPosition;

public partial class RemoveDepartmentPositionCommandHandler
    : ICommandHandler<RemoveDepartmentPositionCommand, UnitResult<ErrorList>>
{
    private readonly IDepartmentsRepository _departmentsRepository;
    private readonly IPositionsRepository _positionsRepository;
    private readonly ITransactionManager _transactionManager;
    private readonly ILogger<RemoveDepartmentPositionCommandHandler> _logger;

    public RemoveDepartmentPositionCommandHandler(
        IDepartmentsRepository departmentsRepository,
        IPositionsRepository positionsRepository,
        ITransactionManager transactionManager,
        ILogger<RemoveDepartmentPositionCommandHandler> logger)
    {
        _departmentsRepository = departmentsRepository;
        _positionsRepository = positionsRepository;
        _transactionManager = transactionManager;
        _logger = logger;
    }

    public async Task<UnitResult<ErrorList>> Handle(
        RemoveDepartmentPositionCommand command, CancellationToken cancellationToken)
    {
        var positionResult = await _positionsRepository.GetByIdAsync(command.PositionId, cancellationToken);
        if (positionResult.IsFailure)
            return positionResult.Error;

        var departmentResult = await _departmentsRepository.GetByIdAsync(command.DepartmentId, cancellationToken);
        if (departmentResult.IsFailure)
            return departmentResult.Error;

        var linkExists = await _departmentsRepository.PositionLinkExistsAsync(
            command.DepartmentId, command.PositionId, cancellationToken);
        if (!linkExists)
        {
            return Error.NotFound("department_position.not_found",
                $"Position '{command.PositionId}' is not linked to department '{command.DepartmentId}'").ToErrors();
        }

        var stageResult = await _departmentsRepository.RemovePositionAsync(
            command.DepartmentId, command.PositionId, cancellationToken);
        if (stageResult.IsFailure)
            return stageResult.Error;

        var result = await _transactionManager.SaveChangesAsync(cancellationToken);
        if (result.IsSuccess)
            LogPositionRemovedFromDepartment(command.DepartmentId, command.PositionId);

        return result;
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Position {PositionId} removed from department {DepartmentId}")]
    private partial void LogPositionRemovedFromDepartment(Guid departmentId, Guid positionId);
}
