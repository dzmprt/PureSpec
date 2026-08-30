using Books.Application.Exceptions;
using Books.Domain;
using Books.Domain.Specifications;
using Microsoft.EntityFrameworkCore;
using MitMediator;

namespace Books.Application.UseCase.Genres.Commands.DeleteGenre;

/// <summary>
/// Handler for <see cref="DeleteGenreCommand"/>.
/// </summary>
internal sealed class DeleteGenreCommandHandler(DbContext dbContext) : IRequestHandler<DeleteGenreCommand>
{
    /// <inheritdoc/>
    public async ValueTask<Unit> HandleAsync(DeleteGenreCommand command, CancellationToken cancellationToken)
    {
        var spec = new GenreByNameSpec(command.GenreName.Trim().ToUpperInvariant());

        var genre = await dbContext.Set<Genre>().FirstOrDefaultAsync(spec.Predicate, cancellationToken);
        if (genre is null)
        {
            throw new NotFoundException();
        }

        dbContext.Set<Genre>().Remove(genre);
        return Unit.Value;
    }
}