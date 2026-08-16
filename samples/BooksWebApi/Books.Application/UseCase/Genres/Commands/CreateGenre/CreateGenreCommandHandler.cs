using Books.Application.Exceptions;
using Books.Domain;
using Books.Domain.Specifications;
using MitMediator;
using PureSpec.Repositories.Abstractions;

namespace Books.Application.UseCase.Genres.Commands.CreateGenre;

/// <summary>
/// Handler for <see cref="CreateGenreCommand"/>.
/// </summary>
internal sealed class CreateGenreCommandHandler(IRepository<Genre> genreRepository) : IRequestHandler<CreateGenreCommand, Genre>
{
    /// <inheritdoc/>
    public async ValueTask<Genre> HandleAsync(CreateGenreCommand command, CancellationToken cancellationToken)
    {
        var query = new GenreByNameSpec(command.GenreName.Trim().ToUpperInvariant()).ToQuery();

        var isGenreExists = await genreRepository.AnyAsync(query, cancellationToken);
        if (isGenreExists)
        {
            throw new BadOperationException($"Genre '{command.GenreName}' already exists.");
        }

        var genre = new Genre(command.GenreName);
        await genreRepository.AddAsync(genre, cancellationToken);
        return genre;
    }
}