using MediatR;
using SoundexDifferenceDemo.Domain.Quotes;

namespace SoundexDifferenceDemo.Application.Quotes.Update;

internal class UpdateQuoteDomainEventHandler : INotificationHandler<UpdateQuoteDomainEvent>
{
    public Task Handle(UpdateQuoteDomainEvent notification, CancellationToken cancellationToken)
    {
        // TODO: Send notification, etc.
        return Task.CompletedTask;
    }
}