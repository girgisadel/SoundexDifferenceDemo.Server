using MediatR;
using SoundexDifferenceDemo.SharedKernel;

namespace SoundexDifferenceDemo.Application.Abstractions.Messaging;

public interface IQuery<TResponse> : IRequest<AppResult<TResponse>>;
