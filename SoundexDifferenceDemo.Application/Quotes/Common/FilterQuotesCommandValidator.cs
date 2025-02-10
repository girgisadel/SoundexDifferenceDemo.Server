using FluentValidation;
using SoundexDifferenceDemo.SharedKernel;

namespace SoundexDifferenceDemo.Application.Quotes.Common;

internal class FilterQuotesCommandValidator : AbstractValidator<FilterQuotesCommand>
{
    public FilterQuotesCommandValidator()
    {
        RuleFor(c => c.SearchTerm)
            .MinimumLength(4)
            .When(c => !string.IsNullOrEmpty(c.SearchTerm))
            .WithMessage("The search term is too short.")
            .WithErrorCode("SearchTermTooShort")

            .MaximumLength(64)
            .When(c => !string.IsNullOrEmpty(c.SearchTerm))
            .WithMessage("The search term is too long.")
            .WithErrorCode("SearchTermTooLong");

        RuleFor(c => c.Page)
            .GreaterThan(0)
            .WithMessage("Page must be greater than 0.")
            .WithErrorCode("PageMustBeGreaterThanZero");

        RuleFor(c => c.PageSize)
            .Must(s => Constants.PageSizeValues.Contains(s))
            .WithMessage("Page size must be one of the valid values.")
            .WithErrorCode("InvalidPageSize");

        RuleFor(c => c.OrderBy)
            .Must(s => Constants.Quotes.OrderByValues.Contains(s))
            .WithMessage("Order by must be one of the valid values.")
            .WithErrorCode("InvalidOrderBy");

        RuleFor(c => c.CreatedAtFrom)
            .LessThanOrEqualTo(c => c.CreatedAtTo)
            .When(c => c.CreatedAtFrom.HasValue && c.CreatedAtTo.HasValue)
            .WithMessage("From date must be earlier than or equal to To date.")
            .WithErrorCode("InvalidDateRange");

        RuleFor(c => c.CreatedAtTo)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .When(c => c.CreatedAtTo.HasValue)
            .WithMessage("To date cannot be in the future.")
            .WithErrorCode("CreatedAtToInFuture");
    }
}