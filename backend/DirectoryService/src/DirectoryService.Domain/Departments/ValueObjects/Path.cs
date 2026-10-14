using CSharpFunctionalExtensions;
using DirectoryService.Domain.Common.Errors;

namespace DirectoryService.Domain.Departments.ValueObjects;

public sealed record Path
{
    private const int MinLength = 2;
    public const int MaxLength = 1000;
    private const char Separator = '/';

    public string Value { get; }

    private Path(string value) => Value = value;

    public static Result<Path, ErrorList> CreateRoot(Slug slug)
    {
        ArgumentNullException.ThrowIfNull(slug);

        string value = slug.Value;
        if (!IsValidLength(value))
        {
            return Error.Validation("path.invalid_length",
                $"Path should be between {MinLength} and {MaxLength} characters long").ToErrors();
        }

        return new Path(value);
    }

    public static Result<Path, ErrorList> CreateChild(Path parentPath, Slug slug)
    {
        ArgumentNullException.ThrowIfNull(parentPath);
        ArgumentNullException.ThrowIfNull(slug);

        string value = parentPath.Value + Separator + slug.Value;
        if (!IsValidLength(value))
        {
            return Error.Validation("path.invalid_length",
                $"Path should be between {MinLength} and {MaxLength} characters long").ToErrors();
        }

        return new Path(value);
    }

    private static bool IsValidLength(string value) =>
        value.Length >= MinLength && value.Length <= MaxLength;

    // Восстановление из БД через EF Core HasConversion — доверенные, уже провалидированные
    // данные из инфраструктуры, а не бизнес-операция клиента, поэтому метод остаётся
    // бросающим исключение, а не возвращающим Result.
    public static Path FromValue(string value)
    {
        if (!IsValidLength(value))
            throw new ArgumentException($"Path should be between {MinLength} and {MaxLength} characters long", nameof(value));

        return new Path(value);
    }
}