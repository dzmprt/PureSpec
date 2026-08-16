using Books.Application.Exceptions;
using Books.Domain;
using Books.Domain.Specifications;
using MitMediator;
using PureSpec.Repositories.Abstractions;

namespace Books.Application.UseCase.Books.Commands.UnpublishBook;

/// <summary>
/// Handler for <see cref="DeleteBookCommand"/>.
/// </summary>
internal sealed class UnpublishBookCommandHandler(IRepository<Book> booksRepository) : IRequestHandler<UnpublishBookCommand>
{
    /// <inheritdoc/>
    public async ValueTask<Unit> HandleAsync(UnpublishBookCommand command, CancellationToken cancellationToken)
    {
        var spec = new BookByIdSpec(command.BookId);

        var book = await booksRepository.FirstOrDefaultAsync(spec.ToQuery(), cancellationToken);
        if (book is null)
        {
            throw new NotFoundException();
        }

        if (!new BookIsAvailableSpec().IsSatisfiedBy(book))
        {
            throw new BookIsNotAvailableException(command.BookId);
        }

        book.Unpublish();

        await booksRepository.UpdateAsync(book, cancellationToken);
        return Unit.Value;
    }
}