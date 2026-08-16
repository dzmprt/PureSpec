using System.Linq.Expressions;

namespace PureSpec.Repositories.Abstractions;

/// <summary>
/// Applies a primary or secondary sort to a query.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
public interface IQueryOrder<TEntity>
{
    /// <summary>
    /// Applies the primary sort.
    /// </summary>
    /// <param name="source">The source to sort.</param>
    /// <returns>The ordered query.</returns>
    IOrderedQueryable<TEntity> Apply(IQueryable<TEntity> source);

    /// <summary>
    /// Applies a secondary sort.
    /// </summary>
    /// <param name="source">The already ordered source.</param>
    /// <returns>The ordered query.</returns>
    IOrderedQueryable<TEntity> ApplyThen(IOrderedQueryable<TEntity> source);
}

/// <summary>
/// Sorts entities by a selected key.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
/// <typeparam name="TKey">The sort key type.</typeparam>
public sealed class QueryOrder<TEntity, TKey> : IQueryOrder<TEntity>
{
    /// <summary>
    /// Gets the expression that selects the sort key.
    /// </summary>
    public Expression<Func<TEntity, TKey>> KeySelector { get; }

    /// <summary>
    /// Gets a value indicating whether the sort is descending.
    /// </summary>
    public bool Descending { get; }

    /// <summary>
    /// Initializes a sort criterion.
    /// </summary>
    /// <param name="keySelector">The expression that selects the sort key.</param>
    /// <param name="descending">Whether to sort in descending order.</param>
    public QueryOrder(Expression<Func<TEntity, TKey>> keySelector, bool descending = false)
    {
        ArgumentNullException.ThrowIfNull(keySelector, nameof(keySelector));
        KeySelector = keySelector;
        Descending = descending;
    }

    /// <summary>
    /// Applies this criterion as the primary sort.
    /// </summary>
    /// <param name="source">The source to sort.</param>
    /// <returns>The ordered query.</returns>
    public IOrderedQueryable<TEntity> Apply(IQueryable<TEntity> source) =>
        Descending
            ? source.OrderByDescending(KeySelector)
            : source.OrderBy(KeySelector);

    /// <summary>
    /// Applies this criterion as a secondary sort.
    /// </summary>
    /// <param name="source">The already ordered source.</param>
    /// <returns>The ordered query.</returns>
    public IOrderedQueryable<TEntity> ApplyThen(IOrderedQueryable<TEntity> source) =>
        Descending
            ? source.ThenByDescending(KeySelector)
            : source.ThenBy(KeySelector);
}