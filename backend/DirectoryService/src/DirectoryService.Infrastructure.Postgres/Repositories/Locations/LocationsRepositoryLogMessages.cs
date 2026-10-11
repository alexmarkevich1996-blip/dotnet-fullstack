using Microsoft.Extensions.Logging;

namespace DirectoryService.Infrastructure.Postgres.Repositories.Locations;

internal static partial class LocationsRepositoryLogMessages
{
    [LoggerMessage(Level = LogLevel.Error, Message = "Failed to save location {LocationId}")]
    public static partial void LogLocationSaveFailed(this ILogger logger, Exception exception, Guid locationId);
}
