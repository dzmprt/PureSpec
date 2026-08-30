using System.Text.Json;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using PureSpec;

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

Console.WriteLine($"Published books count: {await dbContext.Books.CountAsync(publishedSpec.Predicate)}");
Console.WriteLine($"Featured book count: {await dbContext.Books.CountAsync(featured.Predicate)}");
Console.WriteLine($"Book by title \"Dune\": {JsonSerializer.Serialize(await dbContext.Books.SingleOrDefaultAsync(bookByTitleDuneSpec.Predicate))}");

var addedBook = new Book("Neuromancer", "Science fiction", true);
await dbContext.Books.AddAsync(addedBook);
await dbContext.SaveChangesAsync();
var getAddedBookFromDb = await dbContext.Books
    .SingleAsync(new BookByTitleSpec("Neuromancer").Predicate);
Console.WriteLine($"Added book: {JsonSerializer.Serialize(getAddedBookFromDb)}");
Console.WriteLine($"Published count after insert: {await dbContext.Books.CountAsync(publishedSpec.Predicate)}");


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