using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PureSpec.Repositories.Abstractions;
using PureSpec.Repositories.EntityFrameworkCore;

namespace PureSpec.Repositories.EntityFrameworkCore.Tests;

public sealed class EfCoreTests
{
    private sealed class Item
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int Score { get; set; }
    }

    private sealed class TestDbContext(DbContextOptions<TestDbContext> options) : DbContext(options)
    {
        public DbSet<Item> Items => Set<Item>();
    }

    private sealed class NullSelectorQuery : IQuery<Item, string>
    {
        public System.Linq.Expressions.Expression<Func<Item, string>> Selector => null!;
        public System.Linq.Expressions.Expression<Func<Item, bool>>? Predicate => null;
        public int? Limit => null;
        public int? Offset => null;
        public IReadOnlyList<IQueryOrder<Item>> Orderings => [];
        public IQueryable<string> ApplyQuery(IQueryable<Item> source) => throw new NotSupportedException();
    }

    private static (SqliteConnection Connection, TestDbContext Context) CreateContext()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseSqlite(connection)
            .Options;
        var context = new TestDbContext(options);
        context.Database.EnsureCreated();
        context.Items.AddRange(
            new Item { Name = "one", Score = 1 },
            new Item { Name = "two", Score = 2 },
            new Item { Name = "three", Score = 3 });
        context.SaveChanges();
        return (connection, context);
    }

    [Fact]
    public async Task ProviderExecutesSpecificationAndProjectionOperations()
    {
        var (connection, context) = CreateContext();
        await using (connection)
        await using (context)
        {
            var provider = new BaseProvider<Item>(context);
            var specification = new PureSpec.Specification<Item>(item => item.Score > 1);
            var query = specification.ToQuery(item => item.Score, descending: true, limit: 1, offset: 0);
            var projected = new Query<Item, string>(item => item.Score > 1, item => item.Name);

            Assert.Equal(2, await provider.CountAsync(specification.ToQuery(), CancellationToken.None));
            Assert.Equal("three", (await provider.FirstOrDefaultAsync(query, CancellationToken.None))!.Name);
            Assert.Equal("two", (await provider.SingleOrDefaultAsync(new Query<Item>(item => item.Score == 2), CancellationToken.None))!.Name);
            Assert.Equal("two", await provider.FirstOrDefaultAsync(new Query<Item, string>(item => item.Score == 2, item => item.Name), CancellationToken.None));
            Assert.Equal("two", await provider.SingleOrDefaultAsync(new Query<Item, string>(item => item.Score == 2, item => item.Name), CancellationToken.None));
            Assert.Equal(["two", "three"], (await provider.ToArrayAsync(specification.ToQuery(), CancellationToken.None)).Select(item => item.Name));
            Assert.Equal(["two", "three"], await provider.ToArrayAsync(projected, CancellationToken.None));
            Assert.True(await provider.AnyAsync(specification.ToQuery(), CancellationToken.None));
            Assert.True(await provider.AnyAsync(projected, CancellationToken.None));
            Assert.False(await provider.AnyAsync(new Query<Item>(item => item.Score > 10), CancellationToken.None));
        }
    }

    [Fact]
    public async Task ProviderSupportsUnspecifiedQueriesAndNoTracking()
    {
        var (connection, context) = CreateContext();
        await using (connection)
        await using (context)
        {
            context.ChangeTracker.Clear();
            var provider = new BaseProvider<Item>(context);

            Assert.Equal(3, (await provider.ToArrayAsync(CancellationToken.None)).Length);
            Assert.NotNull(await provider.FirstOrDefaultAsync(CancellationToken.None));
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                provider.SingleOrDefaultAsync(CancellationToken.None).AsTask());
            Assert.True(await provider.AnyAsync(CancellationToken.None));
            Assert.Empty(context.ChangeTracker.Entries());
        }
    }

    [Fact]
    public async Task ProviderExecutesSingleOperationsAndCanTrackEntities()
    {
        var (connection, context) = CreateContext();
        await using (connection)
        await using (context)
        {
            var provider = new BaseProvider<Item>(context, useAsNoTracing: false);
            var query = new Query<Item>(item => item.Score == 2);
            var projectedQuery = new Query<Item, string>(item => item.Score == 2, item => item.Name);

            Assert.Equal(3, (await provider.ToArrayAsync(CancellationToken.None)).Length);
            Assert.NotNull(await provider.FirstOrDefaultAsync(CancellationToken.None));
            await Assert.ThrowsAsync<InvalidOperationException>(() => provider.SingleOrDefaultAsync(CancellationToken.None).AsTask());
            Assert.True(await provider.AnyAsync(CancellationToken.None));
            Assert.Equal("two", (await provider.SingleAsync(query, CancellationToken.None)).Name);
            Assert.Equal("two", await provider.SingleAsync(projectedQuery, CancellationToken.None));
            await Assert.ThrowsAsync<InvalidOperationException>(() => provider.SingleAsync(CancellationToken.None).AsTask());

            context.Items.RemoveRange(context.Items);
            await context.SaveChangesAsync();
            context.Items.Add(new Item { Name = "only", Score = 5 });
            await context.SaveChangesAsync();

            Assert.Equal("only", (await provider.SingleOrDefaultAsync(CancellationToken.None))!.Name);
            Assert.Equal("only", (await provider.SingleAsync(CancellationToken.None)).Name);
            var noTrackingProvider = new BaseProvider<Item>(context);
            Assert.Equal("only", (await noTrackingProvider.SingleAsync(CancellationToken.None)).Name);
            Assert.NotEmpty(context.ChangeTracker.Entries());
        }
    }

    [Fact]
    public async Task ProviderRejectsProjectedQueryWithoutSelector()
    {
        var (connection, context) = CreateContext();
        await using (connection)
        await using (context)
        {
            var provider = new BaseProvider<Item>(context);

            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                provider.FirstOrDefaultAsync(new NullSelectorQuery(), CancellationToken.None).AsTask());
        }
    }

    [Fact]
    public async Task RepositoryAddsUpdatesAndDeletesWithinTransactions()
    {
        var (connection, context) = CreateContext();
        await using (connection)
        await using (context)
        {
            var manager = new TransactionManager(context);
            var repository = new BaseRepository<Item>(context, manager);
            var item = new Item { Name = "new", Score = 4 };

            await repository.AddAsync(item, CancellationToken.None);
            Assert.True(manager.IsTransactionStarted);
            await manager.CommitTransactionAsync(CancellationToken.None);
            Assert.NotEqual(0, item.Id);

            item.Name = "updated";
            await repository.UpdateAsync(item, CancellationToken.None);
            await manager.CommitTransactionAsync(CancellationToken.None);
            Assert.Equal("updated", (await repository.FirstOrDefaultAsync(new Query<Item>(x => x.Id == item.Id), CancellationToken.None))!.Name);

            await repository.DeleteAsync(new Query<Item>(x => x.Id == item.Id), CancellationToken.None);
            await manager.CommitTransactionAsync(CancellationToken.None);
            Assert.Null(await repository.FirstOrDefaultAsync(new Query<Item>(x => x.Id == item.Id), CancellationToken.None));
        }
    }

    [Fact]
    public async Task RepositoryDeleteThrowsWhenEntityIsMissing()
    {
        var (connection, context) = CreateContext();
        await using (connection)
        await using (context)
        {
            var repository = new BaseRepository<Item>(context, new TransactionManager(context));

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                repository.DeleteAsync(new Query<Item>(item => item.Id == 999), CancellationToken.None).AsTask());
        }
    }

    [Fact]
    public async Task TransactionManagerHandlesMissingAndActiveTransactions()
    {
        var (connection, context) = CreateContext();
        await using (connection)
        await using (context)
        {
            var manager = new TransactionManager(context);
            await manager.CommitTransactionAsync(CancellationToken.None);
            await manager.RollbackTransactionAsync();
            Assert.False(manager.IsTransactionStarted);

            await manager.BeginTransactionAsync(CancellationToken.None);
            await manager.BeginTransactionAsync(CancellationToken.None);
            Assert.True(manager.IsTransactionStarted);
            await manager.RollbackTransactionAsync();
            Assert.False(manager.IsTransactionStarted);
        }
    }

    [Fact]
    public void DependencyInjectionRegistersProviderRepositoryAndTransactionManager()
    {
        var services = new ServiceCollection();
        services.AddPureSpecRepositories();

        Assert.Contains(services, descriptor => descriptor.ServiceType == typeof(IProvider<>));
        Assert.Contains(services, descriptor => descriptor.ServiceType == typeof(IRepository<>));
        Assert.Contains(services, descriptor => descriptor.ServiceType == typeof(ITransactionManager));
    }
}
