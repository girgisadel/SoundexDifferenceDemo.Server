using MediatR;
using SoundexDifferenceDemo.SharedKernel;

namespace SoundexDifferenceDemo.Application.Abstractions.Messaging;

public interface ICommand : IRequest<AppResult>, IBaseCommand;

public interface ICommand<TResponse> : IRequest<AppResult<TResponse>>, IBaseCommand;

public interface IBaseCommand;
