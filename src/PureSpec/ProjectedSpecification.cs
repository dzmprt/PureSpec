using System.Linq.Expressions;

namespace PureSpec;

/// <summary>
/// Combines an entity filter with a result projection.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
/// <typeparam name="TResult">The projected result type.</typeparam>
public class ProjectedSpecification<TEntity, TResult> : IProjectedSpecification<TEntity, TResult>
{
    /// <summary>
    /// Gets the expression that selects the result.
    /// </summary>
    public Expression<Func<TEntity, TResult>> Selector { get; }

    /// <summary>
    /// Gets the expression that filters entities.
    /// </summary>
    public Expression<Func<TEntity, bool>> Predicate { get; }

    private readonly Lazy<Func<TEntity, bool>> _compiledPredicateFunc;

    /// <summary>
    /// Gets the compiled filtering delegate.
    /// </summary>
    /// <returns>A delegate that evaluates the filter.</returns>
    public Func<TEntity, bool> CompilePredicate() => _compiledPredicateFunc.Value;

    private readonly Lazy<Func<TEntity, TResult>> _compiledSelectorFunc;

    /// <summary>
    /// Gets the compiled projection delegate.
    /// </summary>
    /// <returns>A delegate that creates the projected result.</returns>
    public Func<TEntity, TResult> CompileSelector() => _compiledSelectorFunc.Value;

    /// <summary>
    /// Initializes a projected specification.
    /// </summary>
    /// <param name="specification">The specification that provides the filter.</param>
    /// <param name="selector">The expression that selects the result.</param>
    public ProjectedSpecification(
        ISpecification<TEntity> specification,
        Expression<Func<TEntity, TResult>> selector)
    {
        ArgumentNullException.ThrowIfNull(specification, nameof(specification));
        ArgumentNullException.ThrowIfNull(selector, nameof(selector));

        Selector = selector;
        Predicate = specification.Predicate;
        _compiledPredicateFunc = new Lazy<Func<TEntity, bool>>(() => Predicate.Compile());
        _compiledSelectorFunc = new Lazy<Func<TEntity, TResult>>(() => Selector.Compile());
    }

    /// <summary>
    /// Initializes a projected specification.
    /// </summary>
    /// <param name="predicate">Expression that filters entities.</param>
    /// <param name="selector">The expression that selects the result.</param>
    public ProjectedSpecification(
        Expression<Func<TEntity, bool>> predicate,
        Expression<Func<TEntity, TResult>> selector)
    {
        ArgumentNullException.ThrowIfNull(predicate, nameof(predicate));
        ArgumentNullException.ThrowIfNull(selector, nameof(selector));

        Selector = selector;
        Predicate = predicate;
        _compiledPredicateFunc = new Lazy<Func<TEntity, bool>>(() => Predicate.Compile());
        _compiledSelectorFunc = new Lazy<Func<TEntity, TResult>>(() => Selector.Compile());
    }
}