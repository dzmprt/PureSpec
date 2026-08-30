using Books.Domain;
using Microsoft.EntityFrameworkCore;
using MitMediator;

namespace Books.Application.UseCase.Authors.Commands.CreateAuthor;

/// <summary>
/// Handler for <see cref="CreateAuthorCommand"/>.
/// </summary>
internal sealed class CreateAuthorCommandHandler(DbContext dbContext) : IRequestHandler<CreateAuthorCommand, Author>
{
    /// <inheritdoc/>
    /// <returns>The created author.</returns>
    public async ValueTask<Author> HandleAsync(CreateAuthorCommand command, CancellationToken cancellationToken)
    {
        var author = new Author(command.FirstName, command.LastName);
        await dbContext.Set<Author>().AddAsync(author, cancellationToken);
        return author;
    }
}