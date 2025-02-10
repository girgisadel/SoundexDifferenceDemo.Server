using MediatR;
using SoundexDifferenceDemo.SharedKernel;

namespace SoundexDifferenceDemo.Application.Abstractions.Messaging;

public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, AppResult<TResponse>>
    where TQuery : IQuery<TResponse>;
