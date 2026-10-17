using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Locations;
using DirectoryService.Core.Validation;
using DirectoryService.Domain.Common.Errors;
using DirectoryService.Domain.Common.ValueObjects;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Departments.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Core.Departments;

public partial class DepartmentsService : IDepartmentsService
{
    private readonly IDepartmentsRepository _departmentsRepository;
    private readonly ILocationsRepository _locationsRepository;
    private readonly IValidator<CreateDepartmentDto> _validator;
    private readonly ILogger<DepartmentsService> _logger;

    public DepartmentsService(
        IDepartmentsRepository departmentsRepository,
        ILocationsRepository locationsRepository,
        IValidator<CreateDepartmentDto> validator,
        ILogger<DepartmentsService> logger)
    {
        _departmentsRepository = departmentsRepository;
        _locationsRepository = locationsRepository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<Guid, ErrorList>> Create(CreateDepartmentDto departmentDto, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(departmentDto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToErrorList();
        }

        var nameResult = Name.Create(departmentDto.Name);
        if (nameResult.IsFailure)
            return nameResult.Error;

        var slugResult = Slug.Create(departmentDto.Slug);
        if (slugResult.IsFailure)
            return slugResult.Error;

        var existingDepartment = await _departmentsRepository.GetByNameAsync(nameResult.Value, cancellationToken);
        if (existingDepartment != null)
        {
            return Error.Conflict("department.name.duplicate",
                $"Department with name '{departmentDto.Name}' already exists").ToErrors();
        }

        Department? parentDepartment = null;
        if (departmentDto.ParentId.HasValue)
        {
            var parentResult = await _departmentsRepository.GetByIdAsync(departmentDto.ParentId.Value, cancellationToken);
            if (parentResult.IsFailure)
                return parentResult.Error;

            parentDepartment = parentResult.Value;
        }

        if (departmentDto.LocationIds.Count > 0)
        {
            var existingLocationIds = await _locationsRepository.GetExistingIdsAsync(
                departmentDto.LocationIds, cancellationToken);

            var missingLocationIds = departmentDto.LocationIds
                .Except(existingLocationIds)
                .ToList();

            if (missingLocationIds.Count > 0)
            {
                return ErrorList.From(missingLocationIds
                    .Select(id => Error.NotFound("location.not_found", $"Location '{id}' was not found")));
            }
        }

        var departmentResult = Department.Create(parentDepartment, nameResult.Value, slugResult.Value);
        if (departmentResult.IsFailure)
            return departmentResult.Error;

        var department = departmentResult.Value;

        var departmentLocationsResult = CreateDepartmentLocations(department.Id, departmentDto.LocationIds);
        if (departmentLocationsResult.IsFailure)
            return departmentLocationsResult.Error;

        var saveResult = await _departmentsRepository.AddAsync(department, departmentLocationsResult.Value, cancellationToken);
        if (saveResult.IsFailure)
            return saveResult.Error;

        LogDepartmentCreated(department.Id);

        return department.Id;
    }

    private static Result<List<DepartmentLocation>, ErrorList> CreateDepartmentLocations(
        Guid departmentId, IReadOnlyCollection<Guid> locationIds)
    {
        var departmentLocations = new List<DepartmentLocation>();
        foreach (var locationId in locationIds)
        {
            var result = DepartmentLocation.Create(departmentId, locationId, isPrimary: false);
            if (result.IsFailure)
                return result.Error;

            departmentLocations.Add(result.Value);
        }

        return departmentLocations;
    }

    public async Task<UnitResult<ErrorList>> Update(Guid id, UpdateDepartmentDto departmentDto, CancellationToken cancellationToken)
    {
        var departmentResult = await _departmentsRepository.GetByIdAsync(id, cancellationToken);
        if (departmentResult.IsFailure)
            return departmentResult.Error;

        var nameResult = Name.Create(departmentDto.Name);
        if (nameResult.IsFailure)
            return nameResult.Error;

        departmentResult.Value.UpdateName(nameResult.Value);

        var result = await _departmentsRepository.UpdateAsync(departmentResult.Value, cancellationToken);
        if (result.IsSuccess)
            LogDepartmentUpdated(id);

        return result;
    }

    public async Task<UnitResult<ErrorList>> AddLocation(Guid departmentId, Guid locationId,
        CancellationToken cancellationToken)
    {
        var locationResult = await _locationsRepository.GetByIdAsync(locationId, cancellationToken);
        if (locationResult.IsFailure)
            return locationResult.Error;

        var departmentResult = await _departmentsRepository.GetByIdAsync(departmentId, cancellationToken);
        if (departmentResult.IsFailure)
            return departmentResult.Error;

        var linkExists =
            await _departmentsRepository.LocationLinkExistsAsync(departmentId, locationId, cancellationToken);
        if (linkExists)
        {
            return Error.Conflict("department_location.already_exists",
                $"Location '{locationId}' is already linked to department '{departmentId}'").ToErrors();
        }

        var departmentLocationResult = DepartmentLocation.Create(departmentId, locationId, isPrimary: false);
        if (departmentLocationResult.IsFailure)
            return departmentLocationResult.Error;

        var result = await _departmentsRepository.AddLocationAsync(departmentLocationResult.Value, cancellationToken);
        if (result.IsSuccess)
        {
            LogLocationAddedToDepartment(departmentId, locationId);
        }
        
        return result;
    }

    public async Task<UnitResult<ErrorList>> RemoveLocation(Guid departmentId, Guid locationId, CancellationToken cancellationToken)
    {
        var locationResult = await _locationsRepository.GetByIdAsync(locationId, cancellationToken);
        if (locationResult.IsFailure)
            return locationResult.Error;

        var departmentResult = await _departmentsRepository.GetByIdAsync(departmentId, cancellationToken);
        if (departmentResult.IsFailure)
            return departmentResult.Error;

        var linkExists = await _departmentsRepository.LocationLinkExistsAsync(departmentId, locationId, cancellationToken);
        if (!linkExists)
        {
            return Error.NotFound("department_location.not_found",
                $"Location '{locationId}' is not linked to department '{departmentId}'").ToErrors();
        }

        var result = await _departmentsRepository.RemoveLocationAsync(departmentId, locationId, cancellationToken);
        if(result.IsSuccess)
            LogLocationRemovedFromDepartment(departmentId, locationId);

        return result;
    }

    public async Task<Result<DepartmentResponseDto, ErrorList>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var departmentResult = await _departmentsRepository.GetByIdAsync(id, cancellationToken);
        if(departmentResult.IsFailure)
            return departmentResult.Error;
        
        var department = departmentResult.Value;
        
        return new DepartmentResponseDto(
            department.Id,
            department.Name.Value,
            department.Slug.Value,
            department.Path.Value,
            department.ParentId,
            department.CreatedAt,
            department.UpdatedAt);
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Department created with id {DepartmentId}")]
    private partial void LogDepartmentCreated(Guid departmentId);
    
    [LoggerMessage(Level = LogLevel.Information, Message = "Department {DepartmentId} updated")]
    private partial void LogDepartmentUpdated(Guid departmentId);
    
    [LoggerMessage(Level = LogLevel.Information, Message = "Location {LocationId} added to department {DepartmentId}")]
    private partial void LogLocationAddedToDepartment(Guid departmentId, Guid locationId);
    
    [LoggerMessage(Level = LogLevel.Information, Message = "Location {LocationId} removed from department {DepartmentId}")]
    private partial void LogLocationRemovedFromDepartment(Guid departmentId, Guid locationId);
}
