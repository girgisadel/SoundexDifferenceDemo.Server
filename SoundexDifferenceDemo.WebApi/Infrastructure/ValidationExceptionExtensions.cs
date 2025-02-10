using FluentValidation;
using SoundexDifferenceDemo.SharedKernel;
using System.Collections.Generic;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SoundexDifferenceDemo.WebApi.Infrastructure;

public static class ValidationExceptionExtensions
{
    public static Dictionary<string, string?[]> AsCodeDictionary(this ValidationException exception)
    {
        var groupedErrors = exception.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(g => g.Key, g => g.ToList());

        var errorDictionary = new Dictionary<string, string?[]>();

        foreach (var group in groupedErrors)
        {
            var errors = group.Value.Select(e => e.ErrorCode).ToArray();
            errorDictionary.Add(group.Key, errors);
        }

        return errorDictionary;
    }

    public static Dictionary<string, string?> AsCodeDictionary(this IEnumerable<AppError> errors)
    {
        var errorDictionary = new Dictionary<string, string?>();

        foreach (var error in errors)
        {
            if (!errorDictionary.ContainsKey(error.Code))
            {
                errorDictionary[error.Code] = error.Details;
            }
        }

        return errorDictionary;
    }
}
