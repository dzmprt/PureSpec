using System.Linq.Expressions;

namespace PureSpec;

/// <summary>
/// Represents the composition of a specification and a result projection.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
/// <typeparam name="TResult">The projected result type.</typeparam>
public interface ISpecificationProjection<TEntity, TResult>
{
    /// <summary>
    /// Gets the expression that selects the result.
    /// </summary>
    Expression<Func<TEntity, TResult>> Selector { get; }

    /// <summary>
    /// Gets the expression that filters entities.
    /// </summary>
    Expression<Func<TEntity, bool>> Predicate { get; }

    /// <summary>
    /// Compiles the filtering expression into a delegate.
    /// </summary>
    /// <returns>A delegate that evaluates the filter.</returns>
    Func<TEntity, bool> CompilePredicate();

    /// <summary>
    /// Compiles the projection expression into a delegate.
    /// </summary>
    /// <returns>A delegate that creates the projected result.</returns>
    Func<TEntity, TResult> CompileSelector();
}