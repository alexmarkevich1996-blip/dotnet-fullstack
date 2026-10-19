using System.Diagnostics.CodeAnalysis;

namespace DirectoryService.Core.Abstractions;

[SuppressMessage(
    "Design",
    "CA1040:Avoid empty interfaces",
    Justification = "Marker interface: TResponse is a phantom type parameter used to tie a query to its " +
                     "handler's return type (IQueryHandler<TQuery, TResponse>), the same pattern MediatR's " +
                     "IRequest<TResponse> uses.")]
[SuppressMessage(
    "Design",
    "S2326:Unused type parameters should be removed",
    Justification = "Same reasoning as CA1040: TResponse is intentionally unused within the interface body.")]
public interface IQuery<TResponse>;