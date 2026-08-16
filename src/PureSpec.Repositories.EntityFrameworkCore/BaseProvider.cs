using Microsoft.EntityFrameworkCore;
using PureSpec.Repositories.Abstractions;

namespace PureSpec.Repositories.EntityFrameworkCore;

public class BaseProvider<TEntity> : IProvider<TEntity> where TEntity : class
{
    protected readonly DbContext DbContext;

    protected readonly DbSet<TEntity> DbSet;

    protected readonly bool AsNoTracing;

    public BaseProvider(DbContext dbContext, bool useAsNoTracing = true)
    {
        DbContext = dbContext;
        DbSet = DbContext.Set<TEntity>();
        AsNoTracing = useAsNoTracing;
    }

    public async ValueTask<long> CountAsync(IQuery<TEntity> specQuery, CancellationToken cancellationToken)
    {
        var query = CreateQueryByQuery(specQuery);
        return await query.LongCountAsync(cancellationToken);
    }

    public async ValueTask<long> CountAsync<TResult>(IQuery<TEntity, TResult> specQuery, CancellationToken cancellationToken)
    {
        var query = CreateQueryByQueryWithProjection(specQuery);
        return await query.LongCountAsync(cancellationToken);
    }

    public async ValueTask<long> CountAsync(CancellationToken cancellationToken)
    {
        var queryable = DbSet.AsQueryable();
        return await queryable.LongCountAsync(cancellationToken);
    }

    public async ValueTask<TResult?> FirstOrDefaultAsync<TResult>(IQuery<TEntity, TResult> specQuery, CancellationToken cancellationToken)
    {
        var query = CreateQueryByQueryWithProjection(specQuery);
        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async ValueTask<TEntity?> FirstOrDefaultAsync(IQuery<TEntity> specQuery, CancellationToken cancellationToken)
    {
        var query = CreateQueryByQuery(specQuery);
        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async ValueTask<TResult?> SingleOrDefaultAsync<TResult>(IQuery<TEntity, TResult> specQuery, CancellationToken cancellationToken)
    {
        var query = CreateQueryByQueryWithProjection(specQuery);
        return await query.SingleOrDefaultAsync(cancellationToken);
    }

    public async ValueTask<TEntity?> SingleOrDefaultAsync(IQuery<TEntity> specQuery, CancellationToken cancellationToken)
    {
        var query = CreateQueryByQuery(specQuery);
        return await query.SingleOrDefaultAsync(cancellationToken);
    }

    public async ValueTask<TResult[]> ToArrayAsync<TResult>(IQuery<TEntity, TResult> specQuery, CancellationToken cancellationToken)
    {
        var query = CreateQueryByQueryWithProjection(specQuery);
        return await query.ToArrayAsync(cancellationToken);
    }

    public async ValueTask<TEntity[]> ToArrayAsync(IQuery<TEntity> specQuery, CancellationToken cancellationToken)
    {
        var query = CreateQueryByQuery(specQuery);
        return await query.ToArrayAsync(cancellationToken);
    }

    public async ValueTask<bool> AnyAsync(IQuery<TEntity> specQuery, CancellationToken cancellationToken)
    {
        var query = CreateQueryByQuery(specQuery);
        return await query.AnyAsync(cancellationToken);
    }

    public async ValueTask<bool> AnyAsync<TResult>(IQuery<TEntity, TResult> specQuery, CancellationToken cancellationToken)
    {
        var query = CreateQueryByQueryWithProjection(specQuery);
        return await query.AnyAsync(cancellationToken);
    }

    public async ValueTask<TEntity[]> ToArrayAsync(CancellationToken cancellationToken)
    {
        var query = DbSet.AsQueryable();
        if (AsNoTracing)
        {
            query = query.AsNoTracking();
        }

        return await query.ToArrayAsync(cancellationToken);
    }

    public async ValueTask<TEntity?> FirstOrDefaultAsync(CancellationToken cancellationToken)
    {
        var query = DbSet.AsQueryable();
        if (AsNoTracing)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async ValueTask<TEntity?> SingleOrDefaultAsync(CancellationToken cancellationToken)
    {
        var query = DbSet.AsQueryable();
        if (AsNoTracing)
        {
            query = query.AsNoTracking();
        }

        return await query.SingleOrDefaultAsync(cancellationToken);
    }

    public async ValueTask<bool> AnyAsync(CancellationToken cancellationToken)
    {
        var query = DbSet.AsQueryable();
        if (AsNoTracing)
        {
            query = query.AsNoTracking();
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async ValueTask<TEntity> SingleAsync(IQuery<TEntity> specQuery, CancellationToken cancellationToken)
    {
        var query = CreateQueryByQuery(specQuery);
        return await query.SingleAsync(cancellationToken);
    }

    public async ValueTask<TEntity> SingleAsync(CancellationToken cancellationToken)
    {
        var query = DbSet.AsQueryable();

        if (AsNoTracing)
        {
            query = query.AsNoTracking();
        }

        return await query.SingleAsync(cancellationToken);
    }

    public async ValueTask<TResult> SingleAsync<TResult>(IQuery<TEntity, TResult> specQuery, CancellationToken cancellationToken)
    {
        var query = CreateQueryByQueryWithProjection(specQuery);
        return await query.SingleAsync(cancellationToken);
    }

    private IQueryable<TEntity> CreateQueryByQuery(IQuery<TEntity> query)
    {
        var queryable = DbSet.AsQueryable();

        if (AsNoTracing)
        {
            queryable = queryable.AsNoTracking();
        }

        return query.ApplyQuery(queryable);
    }

    private IQueryable<TResult> CreateQueryByQueryWithProjection<TResult>(IQuery<TEntity, TResult> query)
    {
        if (query.Selector == null)
        {
            throw new ArgumentNullException(nameof(query.Selector));
        }

        var queryable = DbSet.AsQueryable();

        if (AsNoTracing)
        {
            queryable = queryable.AsNoTracking();
        }

        return query.ApplyQuery(queryable);
    }
}