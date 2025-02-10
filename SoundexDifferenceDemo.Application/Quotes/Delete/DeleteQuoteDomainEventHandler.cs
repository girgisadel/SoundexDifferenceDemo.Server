using MediatR;
using SoundexDifferenceDemo.Domain.Quotes;

namespace SoundexDifferenceDemo.Application.Quotes.Delete;

internal class DeleteQuoteDomainEventHandler : INotificationHandler<DeleteQuoteDomainEvent>
{
    public Task Handle(DeleteQuoteDomainEvent notification, CancellationToken cancellationToken)
    {
        // TODO: Send notification, etc.
        return Task.CompletedTask;
    }
}
