using System.Linq.Expressions;

namespace PureSpec.Repositories.Abstractions;

/// <summary>
/// Provides query creation extensions for specifications.
/// </summary>
public static class ISpecificationExtensions
{
    /// <summary>
    /// Creates a projected query with ascending ordering, paging, and the specification filter.
    /// </summary>
    /// <param name="spec">The projected specification.</param><param name="ordering">The sort key.</param><param name="limit">The maximum number of results.</param><param name="offset">The number of results to skip.</param>
    /// <returns>A configured projected query.</returns>
    public static IQuery<TEntity, TResult> ToQuery<TEntity, TResult, TKey>(this IProjectedSpecification<TEntity, TResult> spec, Expression<Func<TEntity, TKey>> ordering, int? limit = null, int? offset = null)
    {
        return new Query<TEntity, TResult>(spec.Predicate, spec.Selector, [new QueryOrder<TEntity, TKey>(ordering, false)], limit, offset);
    }

    /// <summary>
    /// Creates a projected query with configurable ordering, paging, and the specification filter.
    /// </summary>
    /// <param name="spec">The projected specification.</param><param name="ordering">The sort key.</param><param name="descending">Whether to sort descending.</param><param name="limit">The maximum number of results.</param><param name="offset">The number of results to skip.</param>
    /// <returns>A configured projected query.</returns>
    public static IQuery<TEntity, TResult> ToQuery<TEntity, TResult, TKey>(this IProjectedSpecification<TEntity, TResult> spec, Expression<Func<TEntity, TKey>> ordering, bool descending, int? limit = null, int? offset = null)
    {
        return new Query<TEntity, TResult>(spec.Predicate, spec.Selector, [new QueryOrder<TEntity, TKey>(ordering, descending)], limit, offset);
    }

    /// <summary>
    /// Creates a projected query with one ordering criterion and optional paging.
    /// </summary>
    /// <param name="spec">The projected specification.</param><param name="ordering">The ordering criterion.</param><param name="limit">The maximum number of results.</param><param name="offset">The number of results to skip.</param>
    /// <returns>A configured projected query.</returns>
    public static IQuery<TEntity, TResult> ToQuery<TEntity, TResult>(this IProjectedSpecification<TEntity, TResult> spec, IQueryOrder<TEntity> ordering, int? limit = null, int? offset = null)
    {
        return new Query<TEntity, TResult>(spec.Predicate, spec.Selector, [ordering], limit, offset);
    }

    /// <summary>
    /// Creates a projected query with ordering criteria and optional paging.
    /// </summary>
    /// <param name="spec">The projected specification.</param><param name="orderings">The ordering criteria.</param><param name="limit">The maximum number of results.</param><param name="offset">The number of results to skip.</param>
    /// <returns>A configured projected query.</returns>
    public static IQuery<TEntity, TResult> ToQuery<TEntity, TResult>(this IProjectedSpecification<TEntity, TResult> spec, IEnumerable<IQueryOrder<TEntity>> orderings, int? limit = null, int? offset = null)
    {
        return new Query<TEntity, TResult>(spec.Predicate, spec.Selector, orderings, limit, offset);
    }

    /// <summary>
    /// Creates a projected, filtered query with optional paging.
    /// </summary>
    /// <param name="spec">The projected specification.</param><param name="limit">The maximum number of results.</param><param name="offset">The number of results to skip.</param>
    /// <returns>A configured projected query.</returns>
    public static IQuery<TEntity, TResult> ToQuery<TEntity, TResult>(this IProjectedSpecification<TEntity, TResult> spec, int? limit = null, int? offset = null)
    {
        return new Query<TEntity, TResult>(spec.Predicate, spec.Selector, null, limit, offset);
    }

    /// <summary>
    /// Creates a query with ascending ordering, paging, and the specification filter.
    /// </summary>
    /// <param name="spec">The specification.</param><param name="ordering">The sort key.</param><param name="limit">The maximum number of results.</param><param name="offset">The number of results to skip.</param>
    /// <returns>A configured query.</returns>
    public static IQuery<TEntity> ToQuery<TEntity, TKey>(this ISpecification<TEntity> spec, Expression<Func<TEntity, TKey>> ordering, int? limit = null, int? offset = null)
    {
        return new Query<TEntity>(spec.Predicate, [new QueryOrder<TEntity, TKey>(ordering, false)], limit, offset);
    }

    /// <summary>
    /// Creates a query with configurable ordering, paging, and the specification filter.
    /// </summary>
    /// <param name="spec">The specification.</param><param name="ordering">The sort key.</param><param name="descending">Whether to sort descending.</param><param name="limit">The maximum number of results.</param><param name="offset">The number of results to skip.</param>
    /// <returns>A configured query.</returns>
    public static IQuery<TEntity> ToQuery<TEntity, TKey>(this ISpecification<TEntity> spec, Expression<Func<TEntity, TKey>> ordering, bool descending, int? limit = null, int? offset = null)
    {
        return new Query<TEntity>(spec.Predicate, [new QueryOrder<TEntity, TKey>(ordering, descending)], limit, offset);
    }

    /// <summary>
    /// Creates a query with one ordering criterion and optional paging.
    /// </summary>
    /// <param name="spec">The specification.</param><param name="ordering">The ordering criterion.</param><param name="limit">The maximum number of results.</param><param name="offset">The number of results to skip.</param>
    /// <returns>A configured query.</returns>
    public static IQuery<TEntity> ToQuery<TEntity>(this ISpecification<TEntity> spec, IQueryOrder<TEntity> ordering, int? limit = null, int? offset = null)
    {
        return new Query<TEntity>(spec.Predicate, [ordering], limit, offset);
    }

    /// <summary>
    /// Creates a query with ordering criteria and optional paging.
    /// </summary>
    /// <param name="spec">The specification.</param><param name="orderings">The ordering criteria.</param><param name="limit">The maximum number of results.</param><param name="offset">The number of results to skip.</param>
    /// <returns>A configured query.</returns>
    public static IQuery<TEntity> ToQuery<TEntity>(this ISpecification<TEntity> spec, IEnumerable<IQueryOrder<TEntity>> orderings, int? limit = null, int? offset = null)
    {
        return new Query<TEntity>(spec.Predicate, orderings, limit, offset);
    }

    /// <summary>
    /// Creates a filtered query with optional paging.
    /// </summary>
    /// <param name="spec">The specification.</param><param name="limit">The maximum number of results.</param><param name="offset">The number of results to skip.</param>
    /// <returns>A configured query.</returns>
    public static IQuery<TEntity> ToQuery<TEntity>(this ISpecification<TEntity> spec, int? limit = null, int? offset = null)
    {
        return new Query<TEntity>(spec.Predicate, null, limit, offset);
    }
}