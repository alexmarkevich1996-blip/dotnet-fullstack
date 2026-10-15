using DirectoryService.Domain.Common.Errors;

namespace DirectoryService.Web.Contracts;

public sealed record Envelope<T>
{
    public T? Data { get; init; }
    public ErrorList? Errors { get; init; }
}

public static class Envelope
{
    public static Envelope<T> Ok<T>(T data) =>
        new() { Data = data };

    public static Envelope<object?> Fail(ErrorList errors) =>
        new() { Errors = errors };
}