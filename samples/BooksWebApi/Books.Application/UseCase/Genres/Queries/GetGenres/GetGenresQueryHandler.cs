using Books.Domain;
using MitMediator;
using PureSpec.Repositories.Abstractions;

namespace Books.Application.UseCase.Genres.Queries.GetGenres;

/// <summary>
/// Handler for <see cref="GetGenresQuery"/>.
/// </summary>
internal sealed class GetGenresQueryHandler(IProvider<Genre> _genreProvider) : IRequestHandler<GetGenresQuery, Genre[]>
{
    /// <inheritdoc/>
    public ValueTask<Genre[]> HandleAsync(GetGenresQuery request, CancellationToken cancellationToken)
    {
        return _genreProvider.ToArrayAsync(cancellationToken);
    }
}