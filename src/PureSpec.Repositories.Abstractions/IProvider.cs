namespace PureSpec.Repositories.Abstractions;

/// <summary>
/// Defines read operations for an entity source.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
public interface IProvider<TEntity> where TEntity : class
{
    /// <summary>
    /// Returns projected results matching a query.
    /// </summary>
    /// <typeparam name="TResult">The projected result type.</typeparam>
    /// <param name="specQuery">The projected query.</param><param name="cancellationToken">The cancellation token.</param>
    /// <returns>All matching projected results.</returns>
    ValueTask<TResult[]> ToArrayAsync<TResult>(IQuery<TEntity, TResult> specQuery, CancellationToken cancellationToken);

    /// <summary>
    /// Returns entities matching a query.
    /// </summary>
    /// <param name="specQuery">The query.</param><param name="cancellationToken">The cancellation token.</param>
    /// <returns>All matching entities.</returns>
    ValueTask<TEntity[]> ToArrayAsync(IQuery<TEntity> specQuery, CancellationToken cancellationToken);

    /// <summary>
    /// Returns all entities.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>All entities.</returns>
    ValueTask<TEntity[]> ToArrayAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Returns the first projected result or the default value.
    /// </summary>
    /// <typeparam name="TResult">The projected result type.</typeparam>
    /// <param name="specQuery">The projected query.</param><param name="cancellationToken">The cancellation token.</param>
    /// <returns>The first matching result, or <see langword="null"/>.</returns>
    ValueTask<TResult?> FirstOrDefaultAsync<TResult>(IQuery<TEntity, TResult> specQuery, CancellationToken cancellationToken);

    /// <summary>
    /// Returns the first matching entity or the default value.
    /// </summary>
    /// <param name="specQuery">The query.</param><param name="cancellationToken">The cancellation token.</param>
    /// <returns>The first matching entity, or <see langword="null"/>.</returns>
    ValueTask<TEntity?> FirstOrDefaultAsync(IQuery<TEntity> specQuery, CancellationToken cancellationToken);

    /// <summary>
    /// Returns the first entity or the default value.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The first entity, or <see langword="null"/>.</returns>
    ValueTask<TEntity?> FirstOrDefaultAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Returns the only projected result or the default value.
    /// </summary>
    /// <typeparam name="TResult">The projected result type.</typeparam>
    /// <param name="specQuery">The projected query.</param><param name="cancellationToken">The cancellation token.</param>
    /// <returns>The only matching result, or <see langword="null"/>.</returns>
    ValueTask<TResult?> SingleOrDefaultAsync<TResult>(IQuery<TEntity, TResult> specQuery, CancellationToken cancellationToken);

    /// <summary>
    /// Returns the only matching entity or the default value.
    /// </summary>
    /// <param name="specQuery">The query.</param><param name="cancellationToken">The cancellation token.</param>
    /// <returns>The only matching entity, or <see langword="null"/>.</returns>
    ValueTask<TEntity?> SingleOrDefaultAsync(IQuery<TEntity> specQuery, CancellationToken cancellationToken);

    /// <summary>
    /// Returns the only entity or the default value.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The only entity, or <see langword="null"/>.</returns>
    ValueTask<TEntity?> SingleOrDefaultAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Returns the only matching entity value.
    /// </summary>
    /// <typeparam name="TResult">The projected result type.</typeparam>
    /// <param name="specQuery">The query.</param><param name="cancellationToken">The cancellation token.</param>
    /// <returns>The only matching entity or throw exception.</returns>
    ValueTask<TResult> SingleAsync<TResult>(IQuery<TEntity, TResult> specQuery, CancellationToken cancellationToken);

    /// <summary>
    /// Returns the only matching entity value.
    /// </summary>
    /// <param name="specQuery">The query.</param><param name="cancellationToken">The cancellation token.</param>
    /// <returns>The only matching entity or throw exception.</returns>
    ValueTask<TEntity> SingleAsync(IQuery<TEntity> specQuery, CancellationToken cancellationToken);

    /// <summary>
    /// Returns the only entity value.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The only matching entity or throw exception.</returns>
    ValueTask<TEntity> SingleAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Counts entities matching a query.
    /// </summary>
    /// <param name="specQuery">The query.</param><param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of matching entities.</returns>
    ValueTask<long> CountAsync(IQuery<TEntity> specQuery, CancellationToken cancellationToken);

    /// <summary>
    /// Counts entities.
    /// </summary>
    /// <returns>The number of entities.</returns>
    ValueTask<long> CountAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Counts entities matching a query.
    /// </summary>
    /// <typeparam name="TResult">The projected result type.</typeparam>
    /// <param name="specQuery">The query.</param><param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of matching entities.</returns>
    ValueTask<long> CountAsync<TResult>(IQuery<TEntity, TResult> specQuery, CancellationToken cancellationToken);

    /// <summary>
    /// Determines whether a query has any results.
    /// </summary>
    /// <param name="specQuery">The query.</param><param name="cancellationToken">The cancellation token.</param>
    /// <returns><see langword="true"/> when a result exists.</returns>
    ValueTask<bool> AnyAsync(IQuery<TEntity> specQuery, CancellationToken cancellationToken);

    /// <summary>
    /// Determines whether a projected query has any results.
    /// </summary>
    /// <typeparam name="TResult">The projected result type.</typeparam>
    /// <param name="specQuery">The projected query.</param><param name="cancellationToken">The cancellation token.</param>
    /// <returns><see langword="true"/> when a result exists.</returns>
    ValueTask<bool> AnyAsync<TResult>(IQuery<TEntity, TResult> specQuery, CancellationToken cancellationToken);

    /// <summary>
    /// Determines whether any entity exists.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><see langword="true"/> when an entity exists.</returns>
    ValueTask<bool> AnyAsync(CancellationToken cancellationToken);
}