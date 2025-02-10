using SoundexDifferenceDemo.Application.Abstractions.Messaging;
using SoundexDifferenceDemo.Application.Quotes.Common;

namespace SoundexDifferenceDemo.Application.Quotes.Update;

public class UpdateQuoteCommand : ICommand<QuoteIdItem>
{
    public string Id { get; set; } = default!;
    public string? Text { get; set; }
    public string? Author { get; set; }
}
