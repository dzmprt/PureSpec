using System.Linq.Expressions;

namespace PureSpec.Repositories.Abstractions;

/// <summary>
/// Describes a filtered, ordered, paged query with a projection.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
/// <typeparam name="TResult">The projected result type.</typeparam>
public interface IQuery<TEntity, TResult>
{
    /// <summary>
    /// Gets the projection expression.
    /// </summary>
    Expression<Func<TEntity, TResult>> Selector { get; }

    /// <summary>
    /// Gets the filter expression.
    /// </summary>
    Expression<Func<TEntity, bool>>? Predicate { get; }

    /// <summary>
    /// Gets the maximum number of results, if specified.
    /// </summary>
    int? Limit { get; }

    /// <summary>
    /// Gets the number of results to skip, if specified.
    /// </summary>
    int? Offset { get; }

    /// <summary>
    /// Gets the query sort criteria.
    /// </summary>
    IReadOnlyList<IQueryOrder<TEntity>> Orderings { get; }

    /// <summary>
    /// Applies the query to an entity source.
    /// </summary>
    /// <param name="source">The source to query.</param>
    /// <returns>The projected query.</returns>
    IQueryable<TResult> ApplyQuery(IQueryable<TEntity> source);
}

/// <summary>
/// Describes a filtered, ordered, and paged query without a projection.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
public interface IQuery<TEntity>
{
    /// <summary>
    /// Gets the filter expression.
    /// </summary>
    Expression<Func<TEntity, bool>>? Predicate { get; }

    /// <summary>
    /// Gets the maximum number of results, if specified.
    /// </summary>
    int? Limit { get; }

    /// <summary>
    /// Gets the number of results to skip, if specified.
    /// </summary>
    int? Offset { get; }

    /// <summary>
    /// Gets the query sort criteria.
    /// </summary>
    IReadOnlyList<IQueryOrder<TEntity>> Orderings { get; }

    /// <summary>
    /// Applies the query to an entity source.
    /// </summary>
    /// <param name="source">The source to query.</param>
    /// <returns>The filtered query.</returns>
    IQueryable<TEntity> ApplyQuery(IQueryable<TEntity> source);
}