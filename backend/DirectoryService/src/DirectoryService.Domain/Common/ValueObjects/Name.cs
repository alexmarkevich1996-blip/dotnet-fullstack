using CSharpFunctionalExtensions;
using DirectoryService.Domain.Common.Errors;

namespace DirectoryService.Domain.Common.ValueObjects;

public sealed record Name
{
    private const int MinLength = 2;
    public const int MaxLength = 64;
    public string Value { get; }
    private Name(string value) => Value = value;

    public static Result<Name, ErrorList> Create(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        string normalized = value.Trim();

        if (normalized.Length == 0)
            return Error.Validation("name.empty", "Name is empty").ToErrors();

        if (normalized.Length is < MinLength or > MaxLength)
            return Error.Validation("name.invalid_length", $"Name should be between {MinLength} and {MaxLength} characters long").ToErrors();

        return new Name(normalized);
    }

    // Восстановление из БД (EF Core HasConversion) — доверенные, уже провалидированные данные
    // из инфраструктуры, а не бизнес-операция клиента, поэтому остаётся throwing.
    public static Name FromValue(string value)
    {
        if (value.Length < MinLength || value.Length > MaxLength)
            throw new ArgumentException($"Name should be between {MinLength} and {MaxLength} characters long", nameof(value));

        return new Name(value);
    }
}