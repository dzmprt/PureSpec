using Books.Domain;
using Books.Domain.Specifications;
using MitMediator;
using PureSpec.Repositories.Abstractions;

namespace Books.Application.UseCase.Authors.Queries.GetAuthorsByFilter;

/// <summary>
/// Handler for <see cref="GetAuthorsByFilterQuery"/>.
/// </summary>
internal sealed class GetAuthorsByFilterQueryHandler(IProvider<Author> authorProvider) : IRequestHandler<GetAuthorsByFilterQuery, Author[]>
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

        return await authorProvider.ToArrayAsync(spec.ToQuery(request.Limit, request.Offset), cancellationToken);
    }
}