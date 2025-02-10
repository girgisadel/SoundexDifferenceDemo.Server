using System.Text.Json.Serialization;

namespace SoundexDifferenceDemo.SharedKernel;

public class BusinessException(params AppError[] appErrors) : Exception
{
    private readonly List<AppError> _errors = [.. appErrors];

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IEnumerable<AppError> Errors => _errors;
}
