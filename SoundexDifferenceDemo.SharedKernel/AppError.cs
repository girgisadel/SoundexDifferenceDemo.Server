namespace SoundexDifferenceDemo.SharedKernel;

public class AppError(string code, string? details = null)
{
    public string Code { get; } = code;
    public string? Details { get; } = details;

    public override string ToString() => Details is null ? Code : $"{Code}: {Details}";
}
