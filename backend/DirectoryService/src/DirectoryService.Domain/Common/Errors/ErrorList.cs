namespace DirectoryService.Domain.Common.Errors;

public class ErrorList : List<Error>
{
    public ErrorList(IEnumerable<Error> errors) : base(errors)
    {
    }

    public static ErrorList From(IEnumerable<Error> errors) => new(errors);

    public static ErrorList FromError(Error error) => new([error]);

    public static implicit operator ErrorList(Error error) => FromError(error);
}
