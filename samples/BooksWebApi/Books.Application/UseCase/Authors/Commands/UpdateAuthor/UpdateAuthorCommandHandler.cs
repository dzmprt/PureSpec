using Books.Application.Exceptions;
using Books.Domain;
using Books.Domain.Specifications;
using Microsoft.EntityFrameworkCore;
using MitMediator;

namespace Books.Application.UseCase.Authors.Commands.UpdateAuthor;

/// <summary>
/// Handler for <see cref="UpdateAuthorCommand"/>.
/// </summary>
internal sealed class UpdateAuthorCommandHandler(DbContext dbContext) : IRequestHandler<UpdateAuthorCommand, Author>
{
    /// <inheritdoc/>
    /// <returns>The updated author.</returns>
    public async ValueTask<Author> HandleAsync(UpdateAuthorCommand command, CancellationToken cancellationToken)
    {
        var spec = new AuthorByIdSpec(command.AuthorId)
            .AndNot(new AuthorIsDeletedSpec());

        var author = await dbContext.Set<Author>().FirstOrDefaultAsync(spec.Predicate, cancellationToken);
        if (author is null)
        {
            throw new NotFoundException();
        }
        author.UpdateFirstName(command.FirstName);
        author.UpdateLastName(command.LastName);
        return author;
    }
}