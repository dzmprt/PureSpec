using Books.Application.Exceptions;
using Books.Domain;
using Books.Domain.Specifications;
using MitMediator;
using PureSpec.Repositories.Abstractions;

namespace Books.Application.UseCase.Authors.Commands.DeleteAuthor;

/// <summary>
/// Handler for <see cref="DeleteAuthorCommand"/>.
/// </summary>
internal sealed class DeleteAuthorCommandHandler(IRepository<Author> authorRepository) : IRequestHandler<DeleteAuthorCommand>
{
    /// <inheritdoc/>
    public async ValueTask<Unit> HandleAsync(DeleteAuthorCommand command, CancellationToken cancellationToken)
    {
        var spec = new AuthorByIdSpec(command.AuthorId)
            .AndNot(new AuthorIsDeletedSpec());

        var author = await authorRepository.FirstOrDefaultAsync(spec.ToQuery(), cancellationToken);
        if (author is null)
        {
            throw new NotFoundException();
        }

        author.Delete();
        await authorRepository.UpdateAsync(author, cancellationToken);
        return Unit.Value;
    }
}