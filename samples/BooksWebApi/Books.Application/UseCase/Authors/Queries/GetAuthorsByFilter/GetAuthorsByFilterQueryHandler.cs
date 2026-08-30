using Books.Domain;
using Books.Domain.Specifications;
using Microsoft.EntityFrameworkCore;
using MitMediator;

namespace Books.Application.UseCase.Authors.Queries.GetAuthorsByFilter;

/// <summary>
/// Handler for <see cref="GetAuthorsByFilterQuery"/>.
/// </summary>
internal sealed class GetAuthorsByFilterQueryHandler(DbContext dbContext) : IRequestHandler<GetAuthorsByFilterQuery, Author[]>
{
    /// <inheritdoc/>
    public async ValueTask<Author[]> HandleAsync(GetAuthorsByFilterQuery request, CancellationToken cancellationToken)
    {
        var freeText = request.FreeText?.Trim().ToUpperInvariant();

        var spec = new AuthorIsDeletedSpec().Not();
        if (!string.IsNullOrWhiteSpace(freeText))
        {
            spec = spec.And(new AuthorByFreeTextSpec(freeText));
        }

        var query = dbContext.Set<Author>()
            .AsNoTracking()
            .Where(spec.Predicate);

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