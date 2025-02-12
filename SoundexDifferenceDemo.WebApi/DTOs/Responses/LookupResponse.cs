namespace SoundexDifferenceDemo.WebApi.DTOs.Responses;

public class LookupResponse<TKey, TValue>(TKey key, TValue value)
{
    public TKey Key { get; set; } = key;
    public TValue Value { get; set; } = value;
}
