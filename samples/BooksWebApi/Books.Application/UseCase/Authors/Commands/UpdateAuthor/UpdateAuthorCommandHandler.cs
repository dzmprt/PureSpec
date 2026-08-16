using Books.Application.Exceptions;
using Books.Domain;
using Books.Domain.Specifications;
using MitMediator;
using PureSpec.Repositories.Abstractions;

namespace Books.Application.UseCase.Authors.Commands.UpdateAuthor;

/// <summary>
/// Handler for <see cref="UpdateAuthorCommand"/>.
/// </summary>
internal sealed class UpdateAuthorCommandHandler(IRepository<Author> authorRepository) : IRequestHandler<UpdateAuthorCommand, Author>
{
    /// <inheritdoc/>
    /// <returns>The updated author.</returns>
    public async ValueTask<Author> HandleAsync(UpdateAuthorCommand command, CancellationToken cancellationToken)
    {
        var spec = new AuthorByIdSpec(command.AuthorId)
            .AndNot(new AuthorIsDeletedSpec());

        var author = await authorRepository.FirstOrDefaultAsync(spec.ToQuery(), cancellationToken);
        if (author is null)
        {
            throw new NotFoundException();
        }
        author.UpdateFirstName(command.FirstName);
        author.UpdateLastName(command.LastName);
        await authorRepository.UpdateAsync(author, cancellationToken);
        return author;
    }
}