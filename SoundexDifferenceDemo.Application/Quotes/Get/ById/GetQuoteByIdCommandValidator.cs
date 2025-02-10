using FluentValidation;

namespace SoundexDifferenceDemo.Application.Quotes.Get.ById;

internal class GetQuoteByIdCommandValidator : AbstractValidator<GetQuoteByIdCommand>
{
    public GetQuoteByIdCommandValidator()
    {
        RuleFor(c => c.Id)
            .NotNull()
            .WithMessage("The Id is null.")
            .WithErrorCode("IdNullField")

            .NotEmpty()
            .WithMessage("The Id is empty.")
            .WithErrorCode("IdEmptyField")

            .Must(id =>
            {
                if (Guid.TryParse(id, out var guid))
                {
                    return guid != Guid.Empty;
                }
                else
                {
                    return false;
                }
            })
            .WithMessage("The Id must be a valid one.")
            .WithErrorCode("IdInvalid");
    }
}