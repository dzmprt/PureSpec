using System.Text.Json;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using PureSpec;
using PureSpec.Repositories.Abstractions;
using PureSpec.Repositories.EntityFrameworkCore;

var publishedSpec = new PublishedSpec();
var scienceFictionSpec = new ScienceFictionSpec();

var bookByTitleDuneSpec = new BookByTitleSpec("Dune").And(publishedSpec);

var featured = publishedSpec.And(scienceFictionSpec);

// Create and fill db
await using var connection = new SqliteConnection("Data Source=:memory:");
await connection.OpenAsync();

var options = new DbContextOptionsBuilder<SampleDbContext>()
    .UseSqlite(connection)
    .Options;
await using var dbContext = new SampleDbContext(options);
await dbContext.Database.EnsureCreatedAsync();

dbContext.Books.AddRange(
    new Book("Dune", "Science fiction", true),
    new Book("Foundation", "Science fiction", true),
    new Book("The Silmarillion", "Fantasy", true),
    new Book("Unpublished draft", "Science fiction", false));
await dbContext.SaveChangesAsync();

var bookProvider = new BaseProvider<Book>(dbContext);

Console.WriteLine($"Published books count: {await bookProvider.CountAsync(publishedSpec.ToQuery(), CancellationToken.None)}");
Console.WriteLine($"Featured book count: {await bookProvider.CountAsync(featured.ToQuery(), CancellationToken.None)}");
Console.WriteLine($"Book by title \"Dune\": {JsonSerializer.Serialize(await bookProvider.SingleOrDefaultAsync(bookByTitleDuneSpec.ToQuery(), CancellationToken.None))}");

var transactionManager = new TransactionManager(dbContext);
var repository = new BaseRepository<Book>(dbContext, transactionManager);
var addedBook = new Book("Neuromancer", "Science fiction", true);
await repository.AddAsync(addedBook, CancellationToken.None);
await transactionManager.CommitTransactionAsync(CancellationToken.None);
var getAddedBookFromDb = await bookProvider
    .SingleAsync(new BookByTitleSpec("Neuromancer").ToQuery(), CancellationToken.None);
Console.WriteLine($"Added book: {JsonSerializer.Serialize(getAddedBookFromDb)}");
Console.WriteLine($"Published count after insert: {await bookProvider.CountAsync(publishedSpec.ToQuery(), CancellationToken.None)}");


public sealed record Book(string Title, string Genre, bool IsPublished)
{
    public int Id { get; set; }
}

public sealed class SampleDbContext(DbContextOptions<SampleDbContext> options) : DbContext(options)
{
    public DbSet<Book> Books => Set<Book>();
}

public class PublishedSpec() :
    Specification<Book>(book => book.IsPublished);

public class ScienceFictionSpec() :
    Specification<Book>(book => book.Genre == "Science fiction");

public class BookByTitleSpec(string title) :
    Specification<Book>(book => book.Title == title);