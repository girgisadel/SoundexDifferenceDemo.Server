using SoundexDifferenceDemo.Application.Abstractions.Messaging;
using SoundexDifferenceDemo.Application.Quotes.Common;

namespace SoundexDifferenceDemo.Application.Quotes.Get.ById;

public class GetQuoteByIdCommand(string id) : ICommand<QuoteItem>
{
    public string Id { get; set; } = id;
}
