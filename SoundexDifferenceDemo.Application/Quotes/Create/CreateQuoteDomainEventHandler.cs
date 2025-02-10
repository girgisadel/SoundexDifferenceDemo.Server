using MediatR;
using SoundexDifferenceDemo.Domain.Quotes;

namespace SoundexDifferenceDemo.Application.Quotes.Create;

internal class CreateQuoteDomainEventHandler : INotificationHandler<CreateQuoteDomainEvent>
{
    public Task Handle(CreateQuoteDomainEvent notification, CancellationToken cancellationToken)
    {
        // TODO: Send notification, etc.
        return Task.CompletedTask;
    }
}