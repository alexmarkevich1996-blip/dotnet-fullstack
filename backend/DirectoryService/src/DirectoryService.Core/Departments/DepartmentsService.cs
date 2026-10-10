using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Locations;
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

    public async Task<Guid> Create(CreateDepartmentDto departmentDto, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(departmentDto, cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var existingDepartment = await _departmentsRepository.GetByNameAsync(departmentDto.Name, cancellationToken);
        if (existingDepartment != null)
        {
            throw DepartmentAlreadyExistsException.ForName(departmentDto.Name);
        }

        var name = Name.Create(departmentDto.Name);
        var slug = Slug.Create(departmentDto.Slug);
        Department? parentDepartment = null;

        if (departmentDto.ParentId.HasValue)
        {
            parentDepartment = await _departmentsRepository.GetByIdAsync(departmentDto.ParentId.Value, cancellationToken);
            if (parentDepartment == null)
                throw DepartmentNotFoundException.ForId(departmentDto.ParentId.Value);
        }

        if (departmentDto.LocationIds.Count > 0)
        {
            var existingLocationIds = await _locationsRepository.GetExistingIdsAsync(
                departmentDto.LocationIds, cancellationToken);

            var missingLocationIds = departmentDto.LocationIds
                .Except(existingLocationIds)
                .ToList();

            if (missingLocationIds.Count > 0)
                throw LocationNotFoundException.ForIds(missingLocationIds);
        }

        var department = Department.Create(parentDepartment, name, slug);

        var departmentLocations = departmentDto.LocationIds
            .Select(locationId => DepartmentLocation.Create(department.Id, locationId, isPrimary: false))
            .ToList();

        await _departmentsRepository.AddAsync(department, departmentLocations, cancellationToken);
        LogDepartmentCreated(department.Id);

        return department.Id;
    }

    public async Task Update(Guid id, UpdateDepartmentDto departmentDto, CancellationToken cancellationToken)
    {
        var existingDepartment =  await _departmentsRepository.GetByIdAsync(id, cancellationToken);
        if (existingDepartment == null)
            throw DepartmentNotFoundException.ForId(id);
        
        var newDepartmentName = Name.Create(departmentDto.Name);
        existingDepartment.UpdateName(newDepartmentName);
        
        await _departmentsRepository.UpdateAsync(existingDepartment, cancellationToken);
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Department created with id {DepartmentId}")]
    private partial void LogDepartmentCreated(Guid departmentId);
}