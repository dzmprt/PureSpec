using System.Linq.Expressions;

namespace PureSpec.Repositories.Abstractions;

/// <summary>
/// Represents a filtered, ordered, and paged entity query.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
public class Query<TEntity> : IQuery<TEntity>
{
    /// <summary>
    /// Gets the filter expression.
    /// </summary>
    public Expression<Func<TEntity, bool>>? Predicate { get; }

    /// <summary>
    /// Gets the maximum number of results, if specified.
    /// </summary>
    public int? Limit { get; }

    /// <summary>
    /// Gets the number of results to skip, if specified.
    /// </summary>
    public int? Offset { get; }

    /// <summary>
    /// Gets the query sort criteria.
    /// </summary>
    public IReadOnlyList<IQueryOrder<TEntity>> Orderings { get; }

    /// <summary>
    /// Initializes an entity query.
    /// </summary>
    /// <param name="predicate">The filter expression.</param>
    /// <param name="orderings">The sort criteria.</param>
    /// <param name="limit">The maximum number of results.</param>
    /// <param name="offset">The number of results to skip.</param>
    public Query(Expression<Func<TEntity, bool>>? predicate = null,
            IEnumerable<IQueryOrder<TEntity>>? orderings = null,
            int? limit = null, int? offset = null)
    {
        if (limit.HasValue && limit.Value < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(limit));
        }
        if (offset.HasValue && offset.Value < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(offset));
        }
        Predicate = predicate;
        Limit = limit;
        Offset = offset;
        Orderings = orderings is null
            ? []
            : Array.AsReadOnly(orderings
                .Select(ordering => ordering ?? throw new ArgumentNullException(nameof(orderings), "Orderings cannot contain null elements."))
                .ToArray());
    }

    /// <summary>
    /// Applies this query to an entity source.
    /// </summary>
    /// <param name="source">The source to query.</param>
    /// <returns>The filtered query.</returns>
    public IQueryable<TEntity> ApplyQuery(IQueryable<TEntity> source)
    {
        ArgumentNullException.ThrowIfNull(source, nameof(source));
        var filtered = Predicate == null ? source : source.Where(Predicate);

        IQueryable<TEntity> ordered = filtered;
        if (Orderings.Count > 0)
        {
            var orderedQuery = Orderings[0].Apply(filtered);
            for (var index = 1; index < Orderings.Count; index++)
            {
                orderedQuery = Orderings[index].ApplyThen(orderedQuery);
            }

            ordered = orderedQuery;
        }

        if (Offset.HasValue)
        {
            ordered = ordered.Skip(Offset.Value);
        }

        if (Limit.HasValue)
        {
            ordered = ordered.Take(Limit.Value);
        }

        return ordered;
    }
}

/// <summary>
/// Represents a filtered, ordered, and paged query with a projection.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
/// <typeparam name="TResult">The projected result type.</typeparam>
public class Query<TEntity, TResult> : IQuery<TEntity, TResult>
{
    /// <summary>
    /// Gets the filter expression.
    /// </summary>
    public Expression<Func<TEntity, bool>>? Predicate { get; }
    /// <summary>
    /// Gets the projection expression.
    /// </summary>
    public Expression<Func<TEntity, TResult>> Selector { get; }
    /// <summary>
    /// Gets the maximum number of results, if specified.
    /// </summary>
    public int? Limit { get; }

    /// <summary>
    /// Gets the number of results to skip, if specified.
    /// </summary>
    public int? Offset { get; }
    /// <summary>
    /// Gets the query sort criteria.
    /// </summary>
    public IReadOnlyList<IQueryOrder<TEntity>> Orderings { get; }

    /// <summary>
    /// Initializes a projected query.
    /// </summary>
    /// <param name="predicate">The filter expression.</param>
    /// <param name="selector">The projection expression.</param>
    /// <param name="orderings">The sort criteria.</param>
    /// <param name="limit">The maximum number of results.</param>
    /// <param name="offset">The number of results to skip.</param>
    public Query(Expression<Func<TEntity, bool>>? predicate, Expression<Func<TEntity, TResult>> selector,
        IEnumerable<IQueryOrder<TEntity>>? orderings = null,
        int? limit = null, int? offset = null)
    {
        ArgumentNullException.ThrowIfNull(selector, nameof(selector));
        if (limit.HasValue && limit.Value < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(limit));
        }
        if (offset.HasValue && offset.Value < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(offset));
        }
        Predicate = predicate;
        Selector = selector;
        Limit = limit;
        Offset = offset;
        Orderings = orderings is null
            ? []
            : Array.AsReadOnly(orderings
                .Select(ordering => ordering ?? throw new ArgumentNullException(nameof(orderings), "Orderings cannot contain null elements."))
                .ToArray());
    }

    /// <summary>
    /// Applies this query to an entity source.
    /// </summary>
    /// <param name="source">The source to query.</param>
    /// <returns>The projected query.</returns>
    public IQueryable<TResult> ApplyQuery(IQueryable<TEntity> source)
    {
        ArgumentNullException.ThrowIfNull(source, nameof(source));
        var filtered = Predicate == null ? source : source.Where(Predicate);

        IQueryable<TEntity> ordered = filtered;
        if (Orderings.Count > 0)
        {
            var orderedQuery = Orderings[0].Apply(filtered);
            for (var index = 1; index < Orderings.Count; index++)
            {
                orderedQuery = Orderings[index].ApplyThen(orderedQuery);
            }

            ordered = orderedQuery;
        }

        if (Offset.HasValue)
        {
            ordered = ordered.Skip(Offset.Value);
        }

        if (Limit.HasValue)
        {
            ordered = ordered.Take(Limit.Value);
        }

        return ordered.Select(Selector);
    }
}