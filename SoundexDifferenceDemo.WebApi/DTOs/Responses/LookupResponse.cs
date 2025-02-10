namespace SoundexDifferenceDemo.WebApi.DTOs.Responses;

public class LookupResponse<T>
{
    public T Key { get; set; }
    public T Value { get; set; }

    public LookupResponse(T key, T value)
    {
        Key = key;
        Value = value;
    }
}
