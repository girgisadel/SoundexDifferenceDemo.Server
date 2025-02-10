using SoundexDifferenceDemo.Application.Abstractions.Messaging;
using SoundexDifferenceDemo.Application.Quotes.Common;

namespace SoundexDifferenceDemo.Application.Quotes.Create;

public class CreateQuoteCommand : ICommand<QuoteIdItem>
{
    public string Text { get; set; } = default!;
    public string Author { get; set; } = default!;
}
