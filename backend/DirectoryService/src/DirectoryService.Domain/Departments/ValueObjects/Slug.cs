using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace DirectoryService.Domain.Departments.ValueObjects;

public partial record Slug
{
    public const int MinLength = 2;
    public const int MaxLength = 100;
    public string Value { get; }

    private Slug(string value) => Value = value;

    [SuppressMessage(
        "Globalization",
        "CA1308:Normalize strings to uppercase",
        Justification = "Slug must be stored lowercase by convention (URLs, tree paths); this is not a case-insensitive comparison.")]
    public static Slug Create(string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        string normalized = value.Trim().ToLowerInvariant();
        
        if(normalized.Length < MinLength || normalized.Length > MaxLength)
            throw new ArgumentException($"slug should be between {MinLength} and {MaxLength} characters long", nameof(value));
        
        if(!SlugPattern().IsMatch(normalized))
            throw new ArgumentException($"lowercase Latin letters, digits, and hyphens only; must not start or end with a hyphen", nameof(value));
        
        return new Slug(normalized);
    }
    
    [GeneratedRegex(@"^[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$", RegexOptions.None, matchTimeoutMilliseconds: 1000)]
    private static partial Regex SlugPattern();
}