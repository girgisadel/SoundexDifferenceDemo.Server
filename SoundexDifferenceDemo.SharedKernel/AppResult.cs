using System.Text.Json.Serialization;

namespace SoundexDifferenceDemo.SharedKernel;

public class AppResult
{
    private readonly List<AppError> _errors;

    protected AppResult()
    {
        Succeeded = true;
        _errors = [];
    }

    protected AppResult(params AppError[] errors)
    {
        Succeeded = false;
        _errors = [.. errors];
    }

    public bool Succeeded { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IEnumerable<AppError> Errors => _errors;

    public static implicit operator AppResult(AppError error) =>
        new([error]);

    public static AppResult Success() => new();

    public static AppResult Failure(params AppError[] errors) => new(errors);

    public override string ToString()
    {
        return Succeeded ? "Succeeded" : string.Join(", ", _errors.Select(e => e.ToString()));
    }
}

public class AppResult<TValue> : AppResult
{
    private readonly TValue? _value;

    private AppResult(TValue value) : base() => _value = value;

    private AppResult(params AppError[] errors) : base(errors) => _value = default;

    public TValue Value => Succeeded
        ? _value!
        : throw new InvalidOperationException("Cannot access the value of a failed result.");

    public static implicit operator AppResult<TValue>(AppError error) =>
        new([error]);

    public static implicit operator AppResult<TValue>(TValue value) =>
        new(value);

    public static AppResult<TValue> Success(TValue value) =>
       new(value);

    public static new AppResult<TValue> Failure(params AppError[] errors) => new(errors);
}
