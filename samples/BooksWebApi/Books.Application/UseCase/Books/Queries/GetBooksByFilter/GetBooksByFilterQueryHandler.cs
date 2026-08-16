using Books.Application.UseCase.Authors.Queries.GetAuthorsByFilter;
using Books.Domain;
using Books.Domain.Specifications;
using MitMediator;
using PureSpec;
using PureSpec.Repositories.Abstractions;

namespace Books.Application.UseCase.Books.Queries.GetBooksByFilter;

/// <summary>
/// Handler for <see cref="GetBooksByFilterQuery"/>.
/// </summary>
internal sealed class GetBooksByFilterQueryHandler(IProvider<Book> booksProvider) : IRequestHandler<GetBooksByFilterQuery, Book[]>
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

        var query = spec.ToQuery(b => b.Title, request.Limit, request.Offset);
        return await booksProvider.ToArrayAsync(query, cancellationToken);
    }
}