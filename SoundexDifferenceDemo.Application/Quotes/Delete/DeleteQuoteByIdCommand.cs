using SoundexDifferenceDemo.Application.Abstractions.Messaging;

namespace SoundexDifferenceDemo.Application.Quotes.Delete;

public class DeleteQuoteByIdCommand(string id) : ICommand
{
    public string Id { get; set; } = id;
}
