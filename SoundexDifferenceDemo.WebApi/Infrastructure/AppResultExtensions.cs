using Microsoft.AspNetCore.Mvc;
using SoundexDifferenceDemo.SharedKernel;

namespace SoundexDifferenceDemo.WebApi.Infrastructure;

public static class AppResultExtensions
{
    public static IActionResult ToIActionResultAndThrowOnFailure<TValue>(this AppResult<TValue> result, 
        Func<TValue, IActionResult>? onSuccess = null)
    {
        onSuccess ??= value => new OkObjectResult(value);
        return result.Succeeded ? onSuccess(result.Value) : throw new BusinessException([.. result.Errors]);
    }

    public static IActionResult ToIActionResultAndThrowOnFailure(this AppResult result,
       Func<IActionResult>? onSuccess = null)
    {
        onSuccess ??= () => new OkResult();
        return result.Succeeded ? onSuccess() : throw new BusinessException([.. result.Errors]);
    }
}