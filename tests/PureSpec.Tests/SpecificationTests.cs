using System.Linq.Expressions;

namespace PureSpec.Tests;

public sealed class SpecificationTests
{
    private sealed record Item(int Value, string Name);

    [Fact]
    public void ConstructorAndEvaluationUsePredicate()
    {
        var specification = new PureSpec.Specification<Item>(item => item.Value > 1);

        Assert.True(specification.IsSatisfiedBy(new Item(2, "two")));
        Assert.False(specification.IsSatisfiedBy(new Item(1, "one")));
        Assert.Same(specification.ToFunc(), specification.ToFunc());
    }

    [Fact]
    public void ConstructorRejectsNullPredicate()
    {
        Assert.Throws<ArgumentNullException>(() => new PureSpec.Specification<Item>(null!));
    }

    [Fact]
    public void LogicalOperationsComposePredicates()
    {
        var positive = new PureSpec.Specification<Item>(item => item.Value > 0);
        var named = new PureSpec.Specification<Item>(item => item.Name == "ok");
        var item = new Item(1, "ok");
        var nestedLambda = new PureSpec.Specification<Item>(item => new[] { item.Value }.Any(value => value == item.Value));

        Assert.True(positive.And(named).IsSatisfiedBy(item));
        Assert.False(positive.And(named).IsSatisfiedBy(new Item(1, "no")));
        Assert.True(positive.Or(named).IsSatisfiedBy(new Item(-1, "ok")));
        Assert.True(positive.OrNot(named).IsSatisfiedBy(new Item(-1, "no")));
        Assert.False(positive.Not().IsSatisfiedBy(item));
        Assert.True(positive.AndNot(named).IsSatisfiedBy(new Item(1, "no")));
        Assert.True(nestedLambda.And(positive).IsSatisfiedBy(item));
    }

    [Fact]
    public void LogicalOperationsRejectNullOtherSpecification()
    {
        var specification = new PureSpec.Specification<Item>(item => item.Value > 0);

        Assert.Throws<ArgumentNullException>(() => specification.And(null!));
        Assert.Throws<ArgumentNullException>(() => specification.Or(null!));
        Assert.Throws<ArgumentNullException>(() => specification.AndNot(null!));
        Assert.Throws<ArgumentNullException>(() => specification.OrNot(null!));
    }

    [Fact]
    public void ProjectCreatesCompiledPredicateAndSelector()
    {
        var specification = new PureSpec.Specification<Item>(item => item.Value > 0)
            .Project(item => item.Name);

        Assert.True(specification.CompilePredicate()(new Item(1, "ok")));
        Assert.Equal("ok", specification.CompileSelector()(new Item(1, "ok")));
        Assert.Equal("Name", ((MemberExpression)specification.Selector.Body).Member.Name);
    }

    [Fact]
    public void ProjectRejectsNullArguments()
    {
        var specification = new PureSpec.Specification<Item>(item => item.Value > 0);
        ISpecification<Item> specNull = null;
        Assert.Throws<ArgumentNullException>(() => specification.Project<string>(null!));

        Assert.Throws<ArgumentNullException>(() => new ProjectedSpecification<Item, string>(specNull, item => item.Name));
        Assert.Throws<ArgumentNullException>(() => new ProjectedSpecification<Item, string>(specification, null!));
    }
}
