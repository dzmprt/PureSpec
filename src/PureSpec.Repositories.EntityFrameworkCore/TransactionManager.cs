using Microsoft.EntityFrameworkCore;
using PureSpec.Repositories.Abstractions;

namespace PureSpec.Repositories.EntityFrameworkCore;

public class TransactionManager(DbContext dbContext) : ITransactionManager
{

    public bool IsTransactionStarted => dbContext.Database.CurrentTransaction != null;

    public async ValueTask BeginTransactionAsync(CancellationToken cancellationToken)
    {
        if (dbContext.Database.CurrentTransaction == null)
        {
            await dbContext.Database.BeginTransactionAsync(cancellationToken);
        }
    }

    public async ValueTask CommitTransactionAsync(CancellationToken cancellationToken)
    {
        if (dbContext.Database.CurrentTransaction != null)
        {
            await dbContext.Database.CommitTransactionAsync(cancellationToken);
        }
    }

    public async ValueTask RollbackTransactionAsync()
    {
        if (dbContext.Database.CurrentTransaction != null)
        {
            await dbContext.Database.RollbackTransactionAsync();
        }
    }
}