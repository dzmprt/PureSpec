using Books.Application.Exceptions;
using Books.Domain;
using Books.Domain.Specifications;
using MitMediator;
using PureSpec;
using PureSpec.Repositories.Abstractions;

namespace Books.Application.UseCase.Authors.Queries.GetAuthor;

/// <summary>
/// Handler for <see cref="GetAuthorQuery"/>.
/// </summary>
internal sealed class GetAuthorQueryHandler(IProvider<Author> authorProvider) : IRequestHandler<GetAuthorQuery, Author>
{
    /// <inheritdoc/>
    public async ValueTask<Author> HandleAsync(GetAuthorQuery query, CancellationToken cancellationToken)
    {
        var spec = new AuthorByIdSpec(query.AuthorId)
             .AndNot(new AuthorIsDeletedSpec());

        var author = await authorProvider.FirstOrDefaultAsync(spec.ToQuery(), cancellationToken);
        if (author is null)
        {
            throw new NotFoundException();
        }

        return author;
    }
}