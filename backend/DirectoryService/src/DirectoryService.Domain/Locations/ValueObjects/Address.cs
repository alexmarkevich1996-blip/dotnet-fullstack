using CSharpFunctionalExtensions;
using DirectoryService.Domain.Common.Errors;

namespace DirectoryService.Domain.Locations.ValueObjects;

public sealed record Address
{
    private const int MinLength = 5;
    public const int MaxLength = 300;

    public string Value { get; }

    private Address(string value) => Value = value;

    public static Result<Address, ErrorList> Create(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        string normalized = value.Trim();

        if (normalized.Length < MinLength || normalized.Length > MaxLength)
            return Error.Validation("address.invalid_length", $"Address should be between {MinLength} and {MaxLength} characters long").ToErrors();

        return new Address(normalized);
    }

    // Восстановление из БД (EF Core HasConversion) — доверенные, уже провалидированные данные
    // из инфраструктуры, а не бизнес-операция клиента, поэтому остаётся throwing.
    public static Address FromValue(string value)
    {
        if (value.Length < MinLength || value.Length > MaxLength)
            throw new ArgumentException($"Address should be between {MinLength} and {MaxLength} characters long", nameof(value));

        return new Address(value);
    }
}