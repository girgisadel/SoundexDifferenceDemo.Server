using SoundexDifferenceDemo.SharedKernel;

namespace SoundexDifferenceDemo.Domain.Quotes;

public record DeleteQuoteDomainEvent(string QuoteId) : IDomainEvent;
