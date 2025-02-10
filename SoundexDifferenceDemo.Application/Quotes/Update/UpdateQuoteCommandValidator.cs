using FluentValidation;

namespace SoundexDifferenceDemo.Application.Quotes.Update;

internal class UpdateQuoteCommandValidator : AbstractValidator<UpdateQuoteCommand>
{
    public UpdateQuoteCommandValidator()
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

        RuleFor(c => c.Text)
            .NotEmpty()
            .When(c => !string.IsNullOrEmpty(c.Text))
            .WithMessage("The text is empty.")
            .WithErrorCode("TextEmptyField")

            .MinimumLength(8)
            .When(c => !string.IsNullOrEmpty(c.Text))
            .WithMessage("The text must be at least 8 characters long.")
            .WithErrorCode("TextTooShort")

            .MaximumLength(256)
            .When(c => !string.IsNullOrEmpty(c.Text))
            .WithMessage("The text cannot be longer than 256 characters.")
            .WithErrorCode("TextTooLong");

        RuleFor(c => c.Author)
            .NotEmpty()
            .When(c => !string.IsNullOrEmpty(c.Author))
            .WithMessage("The author is empty.")
            .WithErrorCode("AuthorEmptyField")

            .MinimumLength(3)
            .When(c => !string.IsNullOrEmpty(c.Author))
            .WithMessage("The author name must be at least 3 characters long.")
            .WithErrorCode("AuthorTooShort")

            .MaximumLength(32)
            .When(c => !string.IsNullOrEmpty(c.Author))
            .WithMessage("The author name cannot be longer than 32 characters.")
            .WithErrorCode("AuthorTooLong");

        RuleFor(c => new { c.Text, c.Author })
            .Must(x => !string.IsNullOrEmpty(x.Text) || !string.IsNullOrEmpty(x.Author))
            .WithMessage("At least one of 'Text' or 'Author' must be provided.")
            .WithName("UpdateQuoteCommand")
            .WithErrorCode("TextOrAuthorRequired");
    }
}