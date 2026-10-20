using CSharpFunctionalExtensions;
using DirectoryService.Core.Abstractions;
using DirectoryService.Domain.Common.Errors;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Core.Locations.DeleteLocation;

public partial class DeleteLocationCommandHandler
    : ICommandHandler<DeleteLocationCommand, UnitResult<ErrorList>>
{
    private readonly ILocationsRepository _locationsRepository;
    private readonly ITransactionManager _transactionManager;
    private readonly ILogger<DeleteLocationCommandHandler> _logger;

    public DeleteLocationCommandHandler(
        ILocationsRepository locationsRepository,
        ITransactionManager transactionManager,
        ILogger<DeleteLocationCommandHandler> logger)
    {
        _locationsRepository = locationsRepository;
        _transactionManager = transactionManager;
        _logger = logger;
    }

    public async Task<UnitResult<ErrorList>> Handle(
        DeleteLocationCommand command, CancellationToken cancellationToken)
    {
        var locationResult = await _locationsRepository.GetByIdAsync(command.LocationId, cancellationToken);
        if (locationResult.IsFailure)
            return locationResult.Error;

        var hasDepartmentLinks = await _locationsRepository.HasDepartmentLinksAsync(command.LocationId, cancellationToken);
        if (hasDepartmentLinks)
        {
            LogLocationHasLinksConflict(command.LocationId);
            return Error.Conflict("location.has_linked_departments",
                $"Location '{command.LocationId}' is linked to one or more departments and cannot be deleted").ToErrors();
        }

        _locationsRepository.Remove(locationResult.Value);

        var result = await _transactionManager.SaveChangesAsync(cancellationToken);
        if (result.IsSuccess)
            LogLocationDeleted(command.LocationId);

        return result;
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Location {LocationId} deleted")]
    private partial void LogLocationDeleted(Guid locationId);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Attempted to delete location {LocationId} with existing department links")]
    private partial void LogLocationHasLinksConflict(Guid locationId);
}
