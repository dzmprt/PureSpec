using MitMediator;
using PureSpec.Repositories.Abstractions;

namespace Books.Application.Behaviors;

public class DatabaseTransactionBehavior<TRequest, TResponse>(ITransactionManager transactionManager) :
    IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async ValueTask<TResponse> HandleAsync(
        TRequest request,
        IRequestHandlerNext<TRequest, TResponse> next,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await next.InvokeAsync(request, cancellationToken);
            await transactionManager.CommitTransactionAsync(cancellationToken);
            return result;
        }
        catch (Exception)
        {
            await transactionManager.RollbackTransactionAsync();
            throw;
        }
    }
}