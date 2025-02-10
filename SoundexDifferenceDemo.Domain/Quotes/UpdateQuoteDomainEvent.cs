using SoundexDifferenceDemo.SharedKernel;

namespace SoundexDifferenceDemo.Domain.Quotes;

public record UpdateQuoteDomainEvent(string QuoteId) : IDomainEvent;
