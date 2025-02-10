using SoundexDifferenceDemo.SharedKernel;

namespace SoundexDifferenceDemo.Domain.Quotes;

public class Quote(string id) : Entity
{
    public virtual string Id { get; } = id;
    public virtual string? Text { get; set; }
    public virtual string? Author { get; set; }
    public virtual string? NormalizedAuthor { get; set; }
    public virtual DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();
    public Quote() : this(Guid.NewGuid().ToString()) { }
}
