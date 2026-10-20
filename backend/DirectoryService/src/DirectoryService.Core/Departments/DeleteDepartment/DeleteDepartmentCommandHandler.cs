using CSharpFunctionalExtensions;
using DirectoryService.Core.Abstractions;
using DirectoryService.Domain.Common.Errors;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Core.Departments.DeleteDepartment;

public partial class DeleteDepartmentCommandHandler
    : ICommandHandler<DeleteDepartmentCommand, UnitResult<ErrorList>>
{
    private readonly IDepartmentsRepository _departmentsRepository;
    private readonly ITransactionManager _transactionManager;
    private readonly ILogger<DeleteDepartmentCommandHandler> _logger;

    public DeleteDepartmentCommandHandler(
        IDepartmentsRepository departmentsRepository,
        ITransactionManager transactionManager,
        ILogger<DeleteDepartmentCommandHandler> logger)
    {
        _departmentsRepository = departmentsRepository;
        _transactionManager = transactionManager;
        _logger = logger;
    }

    public async Task<UnitResult<ErrorList>> Handle(
        DeleteDepartmentCommand command, CancellationToken cancellationToken)
    {
        var departmentResult = await _departmentsRepository.GetByIdAsync(command.DepartmentId, cancellationToken);
        if (departmentResult.IsFailure)
            return departmentResult.Error;

        var hasChildren = await _departmentsRepository.HasChildrenAsync(command.DepartmentId, cancellationToken);
        if (hasChildren)
        {
            LogDepartmentHasChildrenConflict(command.DepartmentId);
            return Error.Conflict("department.has_children",
                $"Department '{command.DepartmentId}' has child departments and cannot be deleted").ToErrors();
        }

        var hasLocationLinks = await _departmentsRepository.HasLocationLinksAsync(command.DepartmentId, cancellationToken);
        if (hasLocationLinks)
        {
            LogDepartmentHasLinksConflict(command.DepartmentId, "locations");
            return Error.Conflict("department.has_linked_locations",
                $"Department '{command.DepartmentId}' has linked locations and cannot be deleted").ToErrors();
        }

        var hasPositionLinks = await _departmentsRepository.HasPositionLinksAsync(command.DepartmentId, cancellationToken);
        if (hasPositionLinks)
        {
            LogDepartmentHasLinksConflict(command.DepartmentId, "positions");
            return Error.Conflict("department.has_linked_positions",
                $"Department '{command.DepartmentId}' has linked positions and cannot be deleted").ToErrors();
        }

        _departmentsRepository.Remove(departmentResult.Value);

        var result = await _transactionManager.SaveChangesAsync(cancellationToken);
        if (result.IsSuccess)
            LogDepartmentDeleted(command.DepartmentId);

        return result;
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Department {DepartmentId} deleted")]
    private partial void LogDepartmentDeleted(Guid departmentId);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Attempted to delete department {DepartmentId} with existing child departments")]
    private partial void LogDepartmentHasChildrenConflict(Guid departmentId);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Attempted to delete department {DepartmentId} with existing {LinkType} links")]
    private partial void LogDepartmentHasLinksConflict(Guid departmentId, string linkType);
}
