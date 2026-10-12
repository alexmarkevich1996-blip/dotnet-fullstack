using System.Diagnostics.CodeAnalysis;

namespace DirectoryService.Domain.Common.Errors;

[SuppressMessage(
    "Naming",
    "CA1716:Identifiers should not match keywords",
    Justification = "'Error' is the intended domain term here (see task spec); this service is not consumed from VB.NET.")]
public sealed record Error
{
    public string Code { get;}
    public string Message { get;}
    public ErrorType Type { get;}

    private Error(string code, string message, ErrorType type)
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