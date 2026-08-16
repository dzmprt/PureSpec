using Books.Application.Exceptions;
using Books.Domain;
using Books.Domain.Specifications;
using MitMediator;
using PureSpec.Repositories.Abstractions;

namespace Books.Application.UseCase.Genres.Commands.DeleteGenre;

/// <summary>
/// Handler for <see cref="DeleteGenreCommand"/>.
/// </summary>
internal sealed class DeleteGenreCommandHandler(IRepository<Genre> genreRepository) : IRequestHandler<DeleteGenreCommand>
{
    /// <inheritdoc/>
    public async ValueTask<Unit> HandleAsync(DeleteGenreCommand command, CancellationToken cancellationToken)
    {
        var query = new GenreByNameSpec(command.GenreName.Trim().ToUpperInvariant()).ToQuery();

        var genre = await genreRepository.FirstOrDefaultAsync(query, cancellationToken);
        if (genre is null)
        {
            throw new NotFoundException();
        }

        await genreRepository.DeleteAsync(query, cancellationToken);
        return Unit.Value;
    }
}