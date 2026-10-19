using CSharpFunctionalExtensions;
using DirectoryService.Core.Abstractions;
using DirectoryService.Domain.Common.Errors;
using DirectoryService.Domain.Common.ValueObjects;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Core.Departments.UpdateDepartment;

public partial class UpdateDepartmentCommandHandler
    : ICommandHandler<UpdateDepartmentCommand, UnitResult<ErrorList>>
{
    private readonly IDepartmentsRepository _departmentsRepository;
    private readonly ILogger<UpdateDepartmentCommandHandler> _logger;

    public UpdateDepartmentCommandHandler(
        IDepartmentsRepository departmentsRepository,
        ILogger<UpdateDepartmentCommandHandler> logger)
    {
        _departmentsRepository = departmentsRepository;
        _logger = logger;
    }

    public async Task<UnitResult<ErrorList>> Handle(
        UpdateDepartmentCommand command, CancellationToken cancellationToken)
    {
        var departmentResult = await _departmentsRepository.GetByIdAsync(
            command.DepartmentId, cancellationToken);
        if (departmentResult.IsFailure)
            return departmentResult.Error;

        var nameResult = Name.Create(command.Name);
        if (nameResult.IsFailure)
            return nameResult.Error;

        departmentResult.Value.UpdateName(nameResult.Value);

        var result = await _departmentsRepository.UpdateAsync(
            departmentResult.Value, cancellationToken);
        if (result.IsSuccess)
            LogDepartmentUpdated(command.DepartmentId);

        return result;
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Department {DepartmentId} updated")]
    private partial void LogDepartmentUpdated(Guid departmentId);
}