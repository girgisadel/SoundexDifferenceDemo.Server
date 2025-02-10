using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SoundexDifferenceDemo.Domain.Quotes;

namespace SoundexDifferenceDemo.Infrastructure.Configurations;

internal class QuoteConfiguration : IEntityTypeConfiguration<Quote>
{
    public void Configure(EntityTypeBuilder<Quote> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Text).HasMaxLength(1024);

        builder.Property(e => e.Author).HasMaxLength(256);
        builder.Property(e => e.NormalizedAuthor).HasMaxLength(256);

        builder.HasIndex(e => e.Text).IsUnique();

        builder.HasIndex(e => e.NormalizedAuthor).IsUnique(false);

        builder.Property(r => r.ConcurrencyStamp).IsConcurrencyToken();
    }
}
