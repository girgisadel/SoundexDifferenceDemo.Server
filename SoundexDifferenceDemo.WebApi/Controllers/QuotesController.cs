using MediatR;
using Microsoft.AspNetCore.Mvc;
using SoundexDifferenceDemo.Application.Quotes.Create;
using SoundexDifferenceDemo.Application.Quotes.Delete;
using SoundexDifferenceDemo.Application.Quotes.Get.ById;
using SoundexDifferenceDemo.Application.Quotes.Update;
using SoundexDifferenceDemo.WebApi.DTOs.Requests;
using SoundexDifferenceDemo.WebApi.Infrastructure;

namespace SoundexDifferenceDemo.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class QuotesController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateQuoteCommand command)
    {
        return (await sender.Send(command)).ToIActionResultAndThrowOnFailure();
    }

    [HttpPut]
    public async Task<IActionResult> UpdateAsync([FromBody] UpdateQuoteCommand command)
    {
        return (await sender.Send(command)).ToIActionResultAndThrowOnFailure();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteByIdAsync([FromRoute] string id)
    {
        return (await sender.Send(new DeleteQuoteByIdCommand(id))).ToIActionResultAndThrowOnFailure();
    }

    [HttpGet("Get-By-Id/{id}")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] string id)
    {
        return (await sender.Send(new GetQuoteByIdCommand(id))).ToIActionResultAndThrowOnFailure();
    }

    [HttpGet("Normal-Text-Filter")]
    public async Task<IActionResult> NormalFilterQuotesAsync([FromQuery] FilterRequest command)
    {
        return (await sender.Send(command.AsNormalFilterQuotesCommand()))
            .ToIActionResultAndThrowOnFailure();
    }

    [HttpGet("Author-Soundex-Filter")]
    public async Task<IActionResult> SoundexFilterQuotesAsync([FromQuery] FilterRequest command)
    {
        return (await sender.Send(command.AsSoundexFilterQuotesCommand()))
            .ToIActionResultAndThrowOnFailure();
    }

    [HttpGet("Fts-Text-Filter")]
    public async Task<IActionResult> FreeTextSearchFilterQuotesAsync([FromQuery] FilterRequest command)
    {
        return (await sender.Send(command.AsFreeTextSearchFilterQuotesCommand()))
            .ToIActionResultAndThrowOnFailure();
    }
}
