using CSharpFunctionalExtensions;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Locations;
using DirectoryService.Domain.Common.Errors;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Core.Departments.RemoveDepartmentLocation;

public partial class RemoveDepartmentLocationCommandHandler
    : ICommandHandler<RemoveDepartmentLocationCommand, UnitResult<ErrorList>>
{
    private readonly IDepartmentsRepository _departmentsRepository;
    private readonly ILocationsRepository _locationsRepository;
    private readonly ILogger<RemoveDepartmentLocationCommandHandler> _logger;

    public RemoveDepartmentLocationCommandHandler(
        IDepartmentsRepository departmentsRepository,
        ILocationsRepository locationsRepository,
        ILogger<RemoveDepartmentLocationCommandHandler> logger)
    {
        _departmentsRepository = departmentsRepository;
        _locationsRepository = locationsRepository;
        _logger = logger;
    }

    public async Task<UnitResult<ErrorList>> Handle(
        RemoveDepartmentLocationCommand command, CancellationToken cancellationToken)
    {
        var locationResult = await _locationsRepository.GetByIdAsync(command.LocationId, cancellationToken);
        if (locationResult.IsFailure)
            return locationResult.Error;

        var departmentResult = await _departmentsRepository.GetByIdAsync(command.DepartmentId, cancellationToken);
        if (departmentResult.IsFailure)
            return departmentResult.Error;

        var linkExists = await _departmentsRepository.LocationLinkExistsAsync(
            command.DepartmentId, command.LocationId, cancellationToken);
        if (!linkExists)
        {
            return Error.NotFound("department_location.not_found",
                $"Location '{command.LocationId}' is not linked to department '{command.DepartmentId}'").ToErrors();
        }

        var result = await _departmentsRepository.RemoveLocationAsync(
            command.DepartmentId, command.LocationId, cancellationToken);
        if (result.IsSuccess)
            LogLocationRemovedFromDepartment(command.DepartmentId, command.LocationId);

        return result;
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Location {LocationId} removed from department {DepartmentId}")]
    private partial void LogLocationRemovedFromDepartment(Guid departmentId, Guid locationId);
}