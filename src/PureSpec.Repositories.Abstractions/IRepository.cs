namespace PureSpec.Repositories.Abstractions;

/// <summary>
/// Defines read and write operations for an entity source.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
public interface IRepository<TEntity> : IProvider<TEntity> where TEntity : class
{
    /// <summary>
    /// Adds an entity and saves the change.
    /// </summary>
    /// <param name="entity">The entity to add.</param><param name="cancellationToken">The cancellation token.</param>
    ValueTask AddAsync(TEntity entity, CancellationToken cancellationToken);

    /// <summary>
    /// Adds entities and saves the change.
    /// </summary>
    /// <param name="entity">The entity to add.</param><param name="cancellationToken">The cancellation token.</param>
    ValueTask AddManyAsync(TEntity[] entity, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes the single entity matched by a query and saves the change.
    /// </summary>
    /// <param name="specification">The query that identifies the entity.</param><param name="cancellationToken">The cancellation token.</param>
    ValueTask DeleteAsync(IQuery<TEntity> specification, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes entities matched by a query and saves the change.
    /// </summary>
    /// <param name="specification">The query that identifies the entity.</param><param name="cancellationToken">The cancellation token.</param>
    /// <returns>Count of deleted items.</returns>
    ValueTask<int> DeleteManyAsync(IQuery<TEntity> specification, CancellationToken cancellationToken);

    /// <summary>
    /// Updates an entity and saves the change.
    /// </summary>
    /// <param name="entity">The entity to update.</param><param name="cancellationToken">The cancellation token.</param>
    ValueTask UpdateAsync(TEntity entity, CancellationToken cancellationToken);

    /// <summary>
    /// Updates entities and saves the change.
    /// </summary>
    /// <param name="entities">The entity to update.</param><param name="cancellationToken">The cancellation token.</param>
    ValueTask UpdateManyAsync(TEntity[] entities, CancellationToken cancellationToken);
}