using Books.Domain;
using Microsoft.EntityFrameworkCore;
using MitMediator;

namespace Books.Application.UseCase.Genres.Queries.GetGenres;

/// <summary>
/// Handler for <see cref="GetGenresQuery"/>.
/// </summary>
internal sealed class GetGenresQueryHandler(DbContext dbContext) : IRequestHandler<GetGenresQuery, Genre[]>
{
    /// <inheritdoc/>
    public async ValueTask<Genre[]> HandleAsync(GetGenresQuery request, CancellationToken cancellationToken)
    {
        return await dbContext.Set<Genre>().AsNoTracking().ToArrayAsync(cancellationToken);
    }
}