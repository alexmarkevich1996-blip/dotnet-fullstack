using CSharpFunctionalExtensions;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Locations;
using DirectoryService.Domain.Common.Errors;
using DirectoryService.Domain.Departments;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Core.Departments.AddDepartmentLocation;

public partial class AddDepartmentLocationCommandHandler
    : ICommandHandler<AddDepartmentLocationCommand, UnitResult<ErrorList>>
{
    private readonly IDepartmentsRepository _departmentsRepository;
    private readonly ILocationsRepository _locationsRepository;
    private readonly ILogger<AddDepartmentLocationCommandHandler> _logger;

    public AddDepartmentLocationCommandHandler(
        IDepartmentsRepository departmentsRepository,
        ILocationsRepository locationsRepository,
        ILogger<AddDepartmentLocationCommandHandler> logger)
    {
        _departmentsRepository = departmentsRepository;
        _locationsRepository = locationsRepository;
        _logger = logger;
    }

    public async Task<UnitResult<ErrorList>> Handle(
        AddDepartmentLocationCommand command, CancellationToken cancellationToken)
    {
        var locationResult = await _locationsRepository.GetByIdAsync(command.LocationId, cancellationToken);
        if (locationResult.IsFailure)
            return locationResult.Error;

        var departmentResult = await _departmentsRepository.GetByIdAsync(command.DepartmentId, cancellationToken);
        if (departmentResult.IsFailure)
            return departmentResult.Error;

        var linkExists = await _departmentsRepository.LocationLinkExistsAsync(
            command.DepartmentId, command.LocationId, cancellationToken);
        if (linkExists)
        {
            LogDepartmentLocationConflict(command.DepartmentId, command.LocationId);
            return Error.Conflict("department_location.already_exists",
                $"Location '{command.LocationId}' is already linked to department '{command.DepartmentId}'").ToErrors();
        }

        var departmentLocationResult = DepartmentLocation.Create(
            command.DepartmentId, command.LocationId, isPrimary: false);
        if (departmentLocationResult.IsFailure)
            return departmentLocationResult.Error;

        var result = await _departmentsRepository.AddLocationAsync(departmentLocationResult.Value, cancellationToken);
        if (result.IsSuccess)
            LogLocationAddedToDepartment(command.DepartmentId, command.LocationId);

        return result;
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Location {LocationId} added to department {DepartmentId}")]
    private partial void LogLocationAddedToDepartment(Guid departmentId, Guid locationId);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Attempted to link already-linked location {LocationId} to department {DepartmentId}")]
    private partial void LogDepartmentLocationConflict(Guid departmentId, Guid locationId);
}