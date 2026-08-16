using Books.Domain;
using MitMediator;
using PureSpec.Repositories.Abstractions;

namespace Books.Application.UseCase.Authors.Commands.CreateAuthor;

/// <summary>
/// Handler for <see cref="CreateAuthorCommand"/>.
/// </summary>
internal sealed class CreateAuthorCommandHandler(IRepository<Author> _authorRepository) : IRequestHandler<CreateAuthorCommand, Author>
{
    /// <inheritdoc/>
    /// <returns>The created author.</returns>
    public async ValueTask<Author> HandleAsync(CreateAuthorCommand command, CancellationToken cancellationToken)
    {
        var author = new Author(command.FirstName, command.LastName);
        await _authorRepository.AddAsync(author, cancellationToken);
        return author;
    }
}