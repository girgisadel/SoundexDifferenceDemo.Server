using SoundexDifferenceDemo.SharedKernel;

namespace SoundexDifferenceDemo.Domain.Quotes;

public record CreateQuoteDomainEvent(string QuoteId) : IDomainEvent;
