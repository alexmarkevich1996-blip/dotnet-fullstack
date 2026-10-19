using CSharpFunctionalExtensions;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Locations;
using DirectoryService.Core.Validation;
using DirectoryService.Domain.Common.Errors;
using DirectoryService.Domain.Common.ValueObjects;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Departments.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Core.Departments.CreateDepartment;

public partial class CreateDepartmentCommandHandler 
    : ICommandHandler<CreateDepartmentCommand, Result<Guid, ErrorList>>

{
    private readonly IDepartmentsRepository _departmentsRepository;
    private readonly ILocationsRepository _locationsRepository;
    private readonly IValidator<CreateDepartmentCommand> _validator;
    private readonly ITransactionManager _transactionManager;
    private readonly ILogger<CreateDepartmentCommandHandler> _logger;

    public CreateDepartmentCommandHandler(
        IDepartmentsRepository departmentsRepository,
        ILocationsRepository locationsRepository,
        IValidator<CreateDepartmentCommand> validator,
        ITransactionManager transactionManager,
        ILogger<CreateDepartmentCommandHandler> logger)
    {
        _departmentsRepository = departmentsRepository;
        _locationsRepository = locationsRepository;
        _validator = validator;
        _transactionManager = transactionManager;
        _logger = logger;
    }
    
    public async Task<Result<Guid, ErrorList>> Handle(
        CreateDepartmentCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToErrorList();
        }

        var nameResult = Name.Create(command.Name);
        if (nameResult.IsFailure)
            return nameResult.Error;

        var slugResult = Slug.Create(command.Slug);
        if (slugResult.IsFailure)
            return slugResult.Error;

        var existingDepartment = await _departmentsRepository.GetByNameAsync(nameResult.Value, cancellationToken);
        if (existingDepartment != null)
        {
            LogDepartmentNameConflict(command.Name);
            return Error.Conflict("department.name.duplicate",
                $"Department with name '{command.Name}' already exists").ToErrors();
        }

        Department? parentDepartment = null;
        if (command.ParentId.HasValue)
        {
            var parentResult = await _departmentsRepository.GetByIdAsync(command.ParentId.Value, cancellationToken);
            if (parentResult.IsFailure)
                return parentResult.Error;

            parentDepartment = parentResult.Value;
        }

        if (command.LocationIds.Count > 0)
        {
            var existingLocationIds = await _locationsRepository.GetExistingIdsAsync(
                command.LocationIds, cancellationToken);

            var missingLocationIds = command.LocationIds
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

        var departmentLocationsResult = CreateDepartmentLocations(department.Id, command.LocationIds);
        if (departmentLocationsResult.IsFailure)
            return departmentLocationsResult.Error;

        var transactionResult = await _transactionManager.ExecuteInTransactionAsync(async ct =>
        {
            var addResult = await _departmentsRepository.AddAsync(department, ct);
            if (addResult.IsFailure)
                return addResult.Error;

            foreach (var departmentLocation in departmentLocationsResult.Value)
            {
                var linkResult = await _departmentsRepository.AddLocationAsync(departmentLocation, ct);
                if (linkResult.IsFailure)
                    return linkResult.Error;
            }

            return Result.Success<Guid, ErrorList>(department.Id);
        }, cancellationToken);

        if (transactionResult.IsSuccess)
            LogDepartmentCreated(department.Id);

        return transactionResult;
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
    
    [LoggerMessage(Level = LogLevel.Information, Message = "Department created with id {DepartmentId}")]
    private partial void LogDepartmentCreated(Guid departmentId);
    
    [LoggerMessage(Level = LogLevel.Warning, Message = "Attempted to create department with duplicate name {DepartmentName}")]
    private partial void LogDepartmentNameConflict(string departmentName);
}