using FluentValidation;

namespace SoundexDifferenceDemo.Application.Quotes.Create;

internal class CreateQuoteCommandValidator : AbstractValidator<CreateQuoteCommand>
{
    public CreateQuoteCommandValidator()
    {
        RuleFor(c => c.Text)
            .NotNull()
            .WithMessage("The text is null.")
            .WithErrorCode("TextNullField")

            .NotEmpty()
            .WithMessage("The text is empty.")
            .WithErrorCode("TextEmptyField")

            .MinimumLength(8)
            .WithMessage("The text must be at least 8 characters long.")
            .WithErrorCode("TextTooShort")

            .MaximumLength(256)
            .WithMessage("The text cannot be longer than 256 characters.")
            .WithErrorCode("TextTooLong");

        RuleFor(c => c.Author)
            .NotNull()
            .WithMessage("The author is null.")
            .WithErrorCode("AuthorNullField")
            
            .NotEmpty()
            .WithMessage("The author is empty.")
            .WithErrorCode("AuthorEmptyField")
            
            .MinimumLength(3)
            .WithMessage("The author name must be at least 3 characters long.")
            .WithErrorCode("AuthorTooShort")
            
            .MaximumLength(32)
            .WithMessage("The author name cannot be longer than 32 characters.")
            .WithErrorCode("AuthorTooLong");
    }
}
