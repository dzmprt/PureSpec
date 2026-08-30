using Books.Application.Exceptions;
using Books.Domain;
using Books.Domain.Specifications;
using Microsoft.EntityFrameworkCore;
using MitMediator;

namespace Books.Application.UseCase.Books.Commands.UnpublishBook;

/// <summary>
/// Handler for <see cref="DeleteBookCommand"/>.
/// </summary>
internal sealed class UnpublishBookCommandHandler(DbContext dbContext) : IRequestHandler<UnpublishBookCommand>
{
    /// <inheritdoc/>
    public async ValueTask<Unit> HandleAsync(UnpublishBookCommand command, CancellationToken cancellationToken)
    {
        var spec = new BookByIdSpec(command.BookId);

        var book = await dbContext.Set<Book>().FirstOrDefaultAsync(spec.Predicate, cancellationToken);
        if (book is null)
        {
            throw new NotFoundException();
        }

        if (!new BookIsAvailableSpec().IsSatisfiedBy(book))
        {
            throw new BookIsNotAvailableException(command.BookId);
        }

        book.Unpublish();
        return Unit.Value;
    }
}