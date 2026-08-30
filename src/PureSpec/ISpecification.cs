using System.Linq.Expressions;

namespace PureSpec;

/// <summary>
/// Defines a boolean rule for an entity and operations for combining rules.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
public interface ISpecification<TEntity>
{
    /// <summary>
    /// Gets the expression that evaluates the rule.
    /// </summary>
    Expression<Func<TEntity, bool>> Predicate { get; }

    /// <summary>
    /// Determines whether an entity satisfies the rule.
    /// </summary>
    /// <param name="entity">The entity to evaluate.</param>
    /// <returns><see langword="true"/> when the entity satisfies the rule.</returns>
    bool IsSatisfiedBy(TEntity entity);

    /// <summary>
    /// Creates a projection that keeps this rule.
    /// </summary>
    /// <typeparam name="TResult">The projected result type.</typeparam>
    /// <param name="selector">The expression that selects the result.</param>
    /// <returns>A composition of this specification and the projection.</returns>
    ISpecificationProjection<TEntity, TResult> Project<TResult>(Expression<Func<TEntity, TResult>> selector);

    /// <summary>
    /// Combines this rule with another rule using logical OR.
    /// </summary>
    /// <param name="other">The rule to combine with this rule.</param>
    /// <returns>A specification that satisfies either rule.</returns>
    ISpecification<TEntity> Or(ISpecification<TEntity> other);

    /// <summary>
    /// Creates a specification that negates this rule.
    /// </summary>
    /// <returns>A specification with the negated rule.</returns>
    ISpecification<TEntity> Not();

    /// <summary>
    /// Combines this rule with another rule using logical AND.
    /// </summary>
    /// <param name="other">The rule to combine with this rule.</param>
    /// <returns>A specification that satisfies both rules.</returns>
    ISpecification<TEntity> And(ISpecification<TEntity> other);

    /// <summary>
    /// Compiles the rule into a delegate.
    /// </summary>
    /// <returns>A delegate that evaluates the rule.</returns>
    Func<TEntity, bool> ToFunc();
}
