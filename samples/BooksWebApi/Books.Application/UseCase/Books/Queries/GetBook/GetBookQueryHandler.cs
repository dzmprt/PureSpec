using Books.Application.Exceptions;
using Books.Domain;
using Books.Domain.Specifications;
using MitMediator;
using PureSpec;
using PureSpec.Repositories.Abstractions;

namespace Books.Application.UseCase.Books.Queries.GetBook;

/// <summary>
/// Handler for <see cref="GetBookQuery"/>.
/// </summary>
internal sealed class GetBookQueryHandler(IProvider<Book> booksProvider) : IRequestHandler<GetBookQuery, Book>
{
    /// <inheritdoc/>
    public async ValueTask<Book> HandleAsync(GetBookQuery query, CancellationToken cancellationToken)
    {
        ISpecification<Book> spec = new BookByIdSpec(query.BookId);
        var book = await booksProvider.FirstOrDefaultAsync(spec.ToQuery(), cancellationToken);
        if (book is null)
        {
            throw new NotFoundException();
        }

        if (!new BookIsAvailableSpec().IsSatisfiedBy(book))
        {
            throw new BookIsNotAvailableException(query.BookId);
        }

        return book;
    }
}