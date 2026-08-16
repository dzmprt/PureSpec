using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PureSpec.Repositories.EntityFrameworkCore;

namespace Books.Infrastructure;

/// <summary>
/// Dependency injection.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Add application services.
    /// </summary>
    /// <param name="services"><see cref="IServiceProvider"/>.</param>
    /// <returns><see cref="IServiceProvider"/></returns>
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services)
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();
        return services
            .AddDbContext<DbContext, ApplicationDbContext>(options => { options.UseSqlite(connection); })
            .AddPureSpecRepositories()
        ;

    }
}