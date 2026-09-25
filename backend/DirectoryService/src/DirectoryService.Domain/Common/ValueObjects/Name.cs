namespace DirectoryService.Domain.Common.ValueObjects;

public sealed record Name
{
    public const int MinLength = 2;
    public const int MaxLength = 64;
    public string Value { get; }
    private Name(string value) => Value = value;

    public static Name Create(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        string normalized = value.Trim();
        
        if (normalized.Length == 0)
            throw new ArgumentException("Name is empty", nameof(value));
        
        if (normalized.Length is < MinLength or > MaxLength)
            throw new ArgumentException($"Name should be between {MinLength} and {MaxLength} characters long", nameof(value));

        return new Name(normalized);
    }
    
}