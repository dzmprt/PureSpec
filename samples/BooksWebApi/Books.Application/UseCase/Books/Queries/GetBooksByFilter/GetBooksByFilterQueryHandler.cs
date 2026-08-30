using Books.Application.UseCase.Authors.Queries.GetAuthorsByFilter;
using Books.Domain;
using Books.Domain.Specifications;
using Microsoft.EntityFrameworkCore;
using MitMediator;
using PureSpec;

namespace Books.Application.UseCase.Books.Queries.GetBooksByFilter;

/// <summary>
/// Handler for <see cref="GetBooksByFilterQuery"/>.
/// </summary>
internal sealed class GetBooksByFilterQueryHandler(DbContext dbContext) : IRequestHandler<GetBooksByFilterQuery, Book[]>
{
    /// <inheritdoc/>
    public async ValueTask<Book[]> HandleAsync(GetBooksByFilterQuery request, CancellationToken cancellationToken)
    {
        var freeText = request.FreeText?.Trim().ToUpperInvariant();
        ISpecification<Book> spec = new BookIsAvailableSpec();
        if (!string.IsNullOrWhiteSpace(freeText))
        {
            spec = spec.And(new BookByFreeTextSpec(freeText));
        }

        IQueryable<Book> query = dbContext.Set<Book>()
            .AsNoTracking()
            .Where(spec.Predicate)
            .OrderBy(book => book.Title);

        if (request.Offset is int offset)
        {
            query = query.Skip(offset);
        }

        if (request.Limit is int limit)
        {
            query = query.Take(limit);
        }

        return await query.ToArrayAsync(cancellationToken);
    }
}