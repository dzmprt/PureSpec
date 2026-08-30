using Books.Application.Exceptions;
using Books.Domain;
using Books.Domain.Specifications;
using Microsoft.EntityFrameworkCore;
using MitMediator;
using PureSpec;

namespace Books.Application.UseCase.Authors.Queries.GetAuthor;

/// <summary>
/// Handler for <see cref="GetAuthorQuery"/>.
/// </summary>
internal sealed class GetAuthorQueryHandler(DbContext dbContext) : IRequestHandler<GetAuthorQuery, Author>
{
    /// <inheritdoc/>
    public async ValueTask<Author> HandleAsync(GetAuthorQuery query, CancellationToken cancellationToken)
    {
        var spec = new AuthorByIdSpec(query.AuthorId)
             .AndNot(new AuthorIsDeletedSpec());

        var author = await dbContext.Set<Author>()
            .AsNoTracking()
            .FirstOrDefaultAsync(spec.Predicate, cancellationToken);
        if (author is null)
        {
            throw new NotFoundException();
        }

        return author;
    }
}