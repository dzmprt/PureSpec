using Books.Application.Exceptions;
using Books.Domain;
using Books.Domain.Specifications;
using Microsoft.EntityFrameworkCore;
using MitMediator;

namespace Books.Application.UseCase.Authors.Commands.DeleteAuthor;

/// <summary>
/// Handler for <see cref="DeleteAuthorCommand"/>.
/// </summary>
internal sealed class DeleteAuthorCommandHandler(DbContext dbContext) : IRequestHandler<DeleteAuthorCommand>
{
    /// <inheritdoc/>
    public async ValueTask<Unit> HandleAsync(DeleteAuthorCommand command, CancellationToken cancellationToken)
    {
        var spec = new AuthorByIdSpec(command.AuthorId)
            .AndNot(new AuthorIsDeletedSpec());

        var author = await dbContext.Set<Author>().FirstOrDefaultAsync(spec.Predicate, cancellationToken);
        if (author is null)
        {
            throw new NotFoundException();
        }

        author.Delete();
        return Unit.Value;
    }
}