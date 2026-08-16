using System.Linq.Expressions;

namespace PureSpec;

internal static class SpecificationExpressionBuilder
{
    public static Expression<Func<TEntity, bool>> Combine<TEntity>(
        Expression<Func<TEntity, bool>> left,
        Expression<Func<TEntity, bool>> right,
        Func<Expression, Expression, BinaryExpression> combiner)
    {
        var parameter = Expression.Parameter(typeof(TEntity), "x");

        var leftBody = new ParameterReplacer(left.Parameters[0], parameter).Visit(left.Body);
        var rightBody = new ParameterReplacer(right.Parameters[0], parameter).Visit(right.Body);

        var combinedBody = combiner(leftBody, rightBody);

        return Expression.Lambda<Func<TEntity, bool>>(combinedBody, parameter);
    }

    public static Expression<Func<TEntity, bool>> Not<TEntity>(
        Expression<Func<TEntity, bool>> expression)
    {
        var parameter = Expression.Parameter(typeof(TEntity), "x");
        var body = new ParameterReplacer(expression.Parameters[0], parameter).Visit(expression.Body);
        var notBody = Expression.Not(body);
        return Expression.Lambda<Func<TEntity, bool>>(notBody, parameter);
    }

    private class ParameterReplacer : ExpressionVisitor
    {
        private readonly ParameterExpression _sourceParameter;
        private readonly ParameterExpression _newParameter;

        public ParameterReplacer(
            ParameterExpression sourceParameter,
            ParameterExpression newParameter)
        {
            _sourceParameter = sourceParameter;
            _newParameter = newParameter;
        }

        protected override Expression VisitParameter(ParameterExpression node) =>
            node == _sourceParameter ? _newParameter : base.VisitParameter(node);
    }
}