namespace DirectoryService.Domain.Departments.ValueObjects;

public sealed record Path
{
    private const int MinLength = 2;
    private const int MaxLength = 1000;
    private const char Separator = '/';

    public string Value { get; }

    private Path(string value) => Value = value;

    public static Path CreateRoot(Slug slug)
    {
        ArgumentNullException.ThrowIfNull(slug);

        string value = slug.Value;
        Validate(value);

        return new Path(value);
    }

    public static Path CreateChild(Path parentPath, Slug slug)
    {
        ArgumentNullException.ThrowIfNull(parentPath);
        ArgumentNullException.ThrowIfNull(slug);

        string value = parentPath.Value + Separator + slug.Value;
        Validate(value);

        return new Path(value);
    }

    private static void Validate(string value)
    {
        if (value.Length < MinLength || value.Length > MaxLength)
            throw new ArgumentException($"Path should be between {MinLength} and {MaxLength} characters long", nameof(value));
    }
}