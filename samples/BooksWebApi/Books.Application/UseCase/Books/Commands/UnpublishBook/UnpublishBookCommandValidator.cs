using Books.Application.UseCase.Authors.Commands.DeleteAuthor;
using FluentValidation;

namespace Books.Application.UseCase.Books.Commands.UnpublishBook;

/// <summary>
/// Validator for <see cref="DeleteBookCommand"/>.
/// </summary>
internal sealed class UnpublishBookCommandValidator : AbstractValidator<UnpublishBookCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteBookCommandValidator"/>.
    /// </summary>
    public UnpublishBookCommandValidator()
    {
        RuleFor(x => x.BookId).GreaterThan(0);
    }
}