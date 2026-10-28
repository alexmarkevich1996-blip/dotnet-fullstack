namespace DirectoryService.Contracts.Locations;

public record LocationResponseDto(
    Guid Id,
    string Name,
    string Address,
    DateTime CreatedAt,
    DateTime UpdatedAt);