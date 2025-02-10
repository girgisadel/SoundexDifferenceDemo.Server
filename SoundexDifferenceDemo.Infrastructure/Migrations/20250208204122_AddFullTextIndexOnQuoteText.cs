using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoundexDifferenceDemo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFullTextIndexOnQuoteText : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.fulltext_catalogs WHERE name = 'QuotesCatalog')
                CREATE FULLTEXT CATALOG QuotesCatalog AS DEFAULT;
            ", suppressTransaction: true);

            migrationBuilder.Sql(@"
                CREATE FULLTEXT INDEX ON Quotes(Text) 
                KEY INDEX PK_Quotes ON QuotesCatalog;
            ", suppressTransaction: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.fulltext_indexes WHERE object_id = OBJECT_ID('Quotes'))
                DROP FULLTEXT INDEX ON Quotes;
            ", suppressTransaction: true);

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.fulltext_catalogs WHERE name = 'QuotesCatalog')
                DROP FULLTEXT CATALOG QuotesCatalog;
            ", suppressTransaction: true);
        }
    }
}
