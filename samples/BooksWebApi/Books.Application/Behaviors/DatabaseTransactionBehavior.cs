using Microsoft.EntityFrameworkCore;
using MitMediator;

namespace Books.Application.Behaviors;

public class DatabaseTransactionBehavior<TRequest, TResponse>(DbContext dbContext) :
    IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async ValueTask<TResponse> HandleAsync(
        TRequest request,
        IRequestHandlerNext<TRequest, TResponse> next,
        CancellationToken cancellationToken)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        var result = await next.InvokeAsync(request, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return result;
    }
}