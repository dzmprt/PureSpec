using Books.Application.Exceptions;
using Books.Domain;
using Books.Domain.Specifications;
using Microsoft.EntityFrameworkCore;
using MitMediator;
using PureSpec;

namespace Books.Application.UseCase.Books.Queries.GetBook;

/// <summary>
/// Handler for <see cref="GetBookQuery"/>.
/// </summary>
internal sealed class GetBookQueryHandler(DbContext dbContext) : IRequestHandler<GetBookQuery, Book>
{
    /// <inheritdoc/>
    public async ValueTask<Book> HandleAsync(GetBookQuery query, CancellationToken cancellationToken)
    {
        ISpecification<Book> spec = new BookByIdSpec(query.BookId);
        var book = await dbContext.Set<Book>()
            .AsNoTracking()
            .FirstOrDefaultAsync(spec.Predicate, cancellationToken);
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