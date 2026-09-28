namespace DirectoryService.Domain.Locations.ValueObjects;

public sealed record Address
{
    private const int MinLength = 5;
    private const int MaxLength = 300;

    public string Value { get; }

    private Address(string value) => Value = value;

    public static Address Create(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        string normalized = value.Trim();

        if (normalized.Length < MinLength || normalized.Length > MaxLength)
            throw new ArgumentException($"Address should be between {MinLength} and {MaxLength} characters long", nameof(value));

        return new Address(normalized);
    }
}