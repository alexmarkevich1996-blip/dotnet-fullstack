namespace DirectoryService.Domain.Common.Errors;

public sealed record Error
{
    public string Code { get;}
    public string Message { get;}
    public ErrorType Type { get;}

    public Error(string code, string message, ErrorType type)
    {
        Code = code;
        Message = message;
        Type = type;
    }
    
    public static Error NotFound(string code, string message) =>
        new(code, message, ErrorType.NotFound);
    public static Error Conflict(string code, string message) =>
        new(code, message, ErrorType.Conflict);
    public static Error Validation(string code, string message) =>
        new(code, message, ErrorType.Validation);
    public static Error Internal(string code, string message) =>
        new(code, message, ErrorType.Internal);
}