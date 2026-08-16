using Microsoft.Extensions.DependencyInjection;
using PureSpec.Repositories.Abstractions;

namespace PureSpec.Repositories.EntityFrameworkCore;

/// <summary>
/// Dependency injection.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Add PureSpec repositories transaction manager and for EntityFrameworkCore:
    /// <see cref="IProvider{}"/> -> <see cref="BaseProvider{}"/>;
    /// <see cref="IRepository{}"/> -> <see cref="BaseRepository{}"/>;
    /// <see cref="ITransactionManager"/> -> <see cref="TransactionManager"/>;
    /// </summary>
    /// <param name="services"><see cref="IServiceProvider"/>.</param>
    /// <returns><see cref="IServiceProvider"/></returns>
    public static IServiceCollection AddPureSpecRepositories(this IServiceCollection services)
    {
        return services
            .AddTransient(typeof(IProvider<>), typeof(BaseProvider<>))
            .AddTransient(typeof(IRepository<>), typeof(BaseRepository<>))
            .AddScoped<ITransactionManager, TransactionManager>();

    }
}