using System.Linq.Expressions;

namespace PureSpec;

/// <summary>
/// Implements a reusable boolean rule for an entity.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
public class Specification<TEntity> : ISpecification<TEntity>
{
    /// <summary>
    /// Gets the expression that evaluates the rule.
    /// </summary>
    public Expression<Func<TEntity, bool>> Predicate { get; }

    /// <summary>
    /// Initializes a specification with a filtering expression.
    /// </summary>
    /// <param name="predicate">The expression that defines the rule.</param>
    public Specification(Expression<Func<TEntity, bool>> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate, nameof(predicate));
        Predicate = predicate;
        _compiledFunc = new Lazy<Func<TEntity, bool>>(() => Predicate.Compile());
    }

    private readonly Lazy<Func<TEntity, bool>> _compiledFunc;

    /// <summary>
    /// Gets the compiled rule delegate.
    /// </summary>
    /// <returns>A delegate that evaluates the rule.</returns>
    public Func<TEntity, bool> ToFunc() => _compiledFunc.Value;

    /// <summary>
    /// Determines whether an entity satisfies the rule.
    /// </summary>
    /// <param name="entity">The entity to evaluate.</param>
    /// <returns><see langword="true"/> when the entity satisfies the rule.</returns>
    public bool IsSatisfiedBy(TEntity entity) => ToFunc()(entity);

    /// <summary>
    /// Combines this rule with another rule using logical OR.
    /// </summary>
    /// <param name="other">The rule to combine with this rule.</param>
    /// <returns>A specification that satisfies either rule.</returns>
    public ISpecification<TEntity> Or(ISpecification<TEntity> other)
    {
        ArgumentNullException.ThrowIfNull(other, nameof(other));
        return new Specification<TEntity>(
                   SpecificationExpressionBuilder.Combine(Predicate, other.Predicate, Expression.OrElse));
    }

    /// <summary>
    /// Combines this rule with the negation of another rule using logical OR.
    /// </summary>
    /// <param name="other">The rule to negate and combine with this rule.</param>
    /// <returns>A specification that satisfies this rule or not the other rule.</returns>
    public ISpecification<TEntity> OrNot(ISpecification<TEntity> other)
    {
        ArgumentNullException.ThrowIfNull(other, nameof(other));
        return new Specification<TEntity>(
                   SpecificationExpressionBuilder.Combine(Predicate, SpecificationExpressionBuilder.Not(other.Predicate), Expression.OrElse));
    }

    /// <summary>
    /// Creates a specification that negates this rule.
    /// </summary>
    /// <returns>A specification with the negated rule.</returns>
    public ISpecification<TEntity> Not()
    {
        return new Specification<TEntity>(SpecificationExpressionBuilder.Not(Predicate));
    }

    /// <summary>
    /// Combines this rule with another rule using logical AND.
    /// </summary>
    /// <param name="other">The rule to combine with this rule.</param>
    /// <returns>A specification that satisfies both rules.</returns>
    public ISpecification<TEntity> And(ISpecification<TEntity> other)
    {
        ArgumentNullException.ThrowIfNull(other, nameof(other));
        return new Specification<TEntity>(
               SpecificationExpressionBuilder.Combine(Predicate, other.Predicate, Expression.AndAlso));
    }

    /// <summary>
    /// Combines this rule with the negation of another rule using logical AND.
    /// </summary>
    /// <param name="other">The rule to negate and combine with this rule.</param>
    /// <returns>A specification that satisfies this rule and not the other rule.</returns>
    public ISpecification<TEntity> AndNot(ISpecification<TEntity> other)
    {
        ArgumentNullException.ThrowIfNull(other, nameof(other));
        return new Specification<TEntity>(
               SpecificationExpressionBuilder.Combine(Predicate, SpecificationExpressionBuilder.Not(other.Predicate), Expression.AndAlso));
    }

    /// <summary>
    /// Creates a projected specification that keeps this rule.
    /// </summary>
    /// <typeparam name="TResult">The projected result type.</typeparam>
    /// <param name="selector">The expression that selects the result.</param>
    /// <returns>A projected specification.</returns>
    public IProjectedSpecification<TEntity, TResult> Project<TResult>(Expression<Func<TEntity, TResult>> selector)
    {
        return new ProjectedSpecification<TEntity, TResult>(this, selector);
    }
}