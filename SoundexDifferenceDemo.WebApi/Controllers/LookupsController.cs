using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoundexDifferenceDemo.Application.Abstractions.Managers;
using SoundexDifferenceDemo.Domain.Quotes;
using SoundexDifferenceDemo.SharedKernel;
using SoundexDifferenceDemo.WebApi.DTOs.Responses;

namespace SoundexDifferenceDemo.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LookupsController(IQuoteManager<Quote> manager) : ControllerBase
{
    [HttpGet("Page-Size-Values")]
    public IActionResult GetPageSizeValues()
    {
        return Ok(Constants.PageSizeValues.Select(v => new LookupResponse<int>(v, v)));
    }

    [HttpGet("Order-By-Values")]
    public IActionResult GetOrderByValuesValues()
    {
        return Ok(Constants.Quotes.OrderByValues.Select(v => new LookupResponse<string>(v, v)));
    }

    [HttpGet("Authors")]
    public async Task<IActionResult> GetAuthorsAsync([FromQuery] string searchTerm)
    {
        var validator = new InlineValidator<string>();

        validator.RuleFor(t => t)
            .NotNull()
            .WithMessage("The search term is null.")
            .WithErrorCode("NullSearchTerm")

            .NotEmpty()
            .WithMessage("The search term is empty.")
            .WithErrorCode("EmptySearchTerm")

            .MinimumLength(4)
            .WithMessage("The search term is too short.")
            .WithErrorCode("SearchTermTooShort")
            
            .MaximumLength(64)
            .WithMessage("The search term is too long.")
            .WithErrorCode("SearchTermTooLong");

        validator.ValidateAndThrow(searchTerm);

        var items = await manager.JustDeferredGetAuthorsSoundexAsync(searchTerm).Select(e => e.Author).ToListAsync();

        return Ok(items.Distinct().Select(e => new LookupResponse<string>(e!, e!)).ToList());
   }
}
