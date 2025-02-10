using MediatR;
using SoundexDifferenceDemo.SharedKernel;

namespace SoundexDifferenceDemo.Application.Abstractions.Messaging;

public interface ICommandHandler<in TCommand>
    : IRequestHandler<TCommand, AppResult>
    where TCommand : ICommand;

public interface ICommandHandler<in TCommand, TResponse>
    : IRequestHandler<TCommand, AppResult<TResponse>>
    where TCommand : ICommand<TResponse>;
