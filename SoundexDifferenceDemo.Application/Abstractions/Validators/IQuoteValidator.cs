using SoundexDifferenceDemo.Application.Abstractions.Managers;
using SoundexDifferenceDemo.Domain.Quotes;
using SoundexDifferenceDemo.SharedKernel;

namespace SoundexDifferenceDemo.Application.Abstractions.Validators;

public interface IQuoteValidator<TQuote> where TQuote : Quote
{
    Task<AppResult> ValidateAsync(IQuoteManager<TQuote> manager, TQuote quote);
}
