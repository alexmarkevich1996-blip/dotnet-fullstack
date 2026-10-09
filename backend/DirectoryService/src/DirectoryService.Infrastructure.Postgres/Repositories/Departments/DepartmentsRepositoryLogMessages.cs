using Microsoft.Extensions.Logging;

namespace DirectoryService.Infrastructure.Postgres.Repositories.Departments;

internal static partial class DepartmentsRepositoryLogMessages
{
    [LoggerMessage(Level = LogLevel.Error, Message = "Failed to save department {departmentId}")]
    public static partial void LogDepartmentsSaveFailed(this ILogger logger, Exception exception, Guid departmentId);
}
