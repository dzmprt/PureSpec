namespace PureSpec.Repositories.Abstractions;

/// <summary>
/// Controls the current database transaction.
/// </summary>
public interface ITransactionManager
{
    /// <summary>
    /// Gets a value indicating whether a transaction is active.
    /// </summary>
    bool IsTransactionStarted { get; }

    /// <summary>
    /// Starts a transaction when one is not already active.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    ValueTask BeginTransactionAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Commits the active transaction.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    ValueTask CommitTransactionAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Rolls back the active transaction.
    /// </summary>
    ValueTask RollbackTransactionAsync();
}