using Microsoft.EntityFrameworkCore;
using PureSpec.Repositories.Abstractions;

namespace PureSpec.Repositories.EntityFrameworkCore;

public class BaseRepository<TEntity>(DbContext dbContext, ITransactionManager transactionManager) :
    BaseProvider<TEntity>(dbContext, false),
    IRepository<TEntity> where TEntity : class
{
    public async ValueTask AddAsync(TEntity entity, CancellationToken cancellationToken)
    {
        DbSet.Add(entity);
        await BeginTransactionIfNotStarted(cancellationToken);
        await DbContext.SaveChangesAsync(cancellationToken);
    }

    public async ValueTask AddManyAsync(TEntity[] entities, CancellationToken cancellationToken)
    {
        DbSet.AddRange(entities);
        await BeginTransactionIfNotStarted(cancellationToken);
        await DbContext.SaveChangesAsync(cancellationToken);
    }

    public async ValueTask DeleteAsync(IQuery<TEntity> specification, CancellationToken cancellationToken)
    {
        var entity = await SingleAsync(specification, cancellationToken);
        DbSet.Remove(entity);
        await BeginTransactionIfNotStarted(cancellationToken);
        await DbContext.SaveChangesAsync(cancellationToken);
    }

    public async ValueTask DeleteManyAsync(IQuery<TEntity> specification, CancellationToken cancellationToken)
    {
        await BeginTransactionIfNotStarted(cancellationToken);
        _ = DbSet.Where(specification.Predicate!).ExecuteDeleteAsync(cancellationToken);
        await DbContext.SaveChangesAsync(cancellationToken);
    }

    public async ValueTask UpdateAsync(TEntity entity, CancellationToken cancellationToken)
    {
        DbSet.Update(entity);
        await BeginTransactionIfNotStarted(cancellationToken);
        await DbContext.SaveChangesAsync(cancellationToken);
    }

    public async ValueTask UpdateManyAsync(TEntity[] entities, CancellationToken cancellationToken)
    {
        DbSet.UpdateRange(entities);
        await BeginTransactionIfNotStarted(cancellationToken);
        await DbContext.SaveChangesAsync(cancellationToken);
    }

    private async ValueTask BeginTransactionIfNotStarted(CancellationToken cancellationToken)
    {
        if (!transactionManager.IsTransactionStarted)
        {
            await transactionManager.BeginTransactionAsync(cancellationToken);
        }
    }
}