using Books.Application.Exceptions;
using Books.Domain;
using Books.Domain.Specifications;
using Microsoft.EntityFrameworkCore;
using MitMediator;

namespace Books.Application.UseCase.Genres.Commands.CreateGenre;

/// <summary>
/// Handler for <see cref="CreateGenreCommand"/>.
/// </summary>
internal sealed class CreateGenreCommandHandler(DbContext dbContext) : IRequestHandler<CreateGenreCommand, Genre>
{
    /// <inheritdoc/>
    public async ValueTask<Genre> HandleAsync(CreateGenreCommand command, CancellationToken cancellationToken)
    {
        var spec = new GenreByNameSpec(command.GenreName.Trim().ToUpperInvariant());

        var isGenreExists = await dbContext.Set<Genre>().AnyAsync(spec.Predicate, cancellationToken);
        if (isGenreExists)
        {
            throw new BadOperationException($"Genre '{command.GenreName}' already exists.");
        }

        var genre = new Genre(command.GenreName);
    await dbContext.Set<Genre>().AddAsync(genre, cancellationToken);
        return genre;
    }
}