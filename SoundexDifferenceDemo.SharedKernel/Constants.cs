namespace SoundexDifferenceDemo.SharedKernel;

public static class Constants
{
    public const int DefaultPage = 1;
    public const int DefaultPageSize = 20;
    public const int MinimumPageSize = 10;
    public const int MaximumPageSize = 100;
    public static int[] PageSizeValues = [MinimumPageSize, DefaultPageSize, 40, 70, MaximumPageSize];

    public static class Quotes
    {
        public const string OrderByAuthor = "Author";
        public const string OrderByText = "Text";
        public const string OrderByCreatedAt = "CreatedAt";
        public static string[] OrderByValues = [OrderByAuthor, OrderByText, OrderByCreatedAt];
    }
}
