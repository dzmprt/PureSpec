using PureSpec;

var publishedSpec = new Specification<Book>(book => book.IsPublished);
var scienceFictionSpec = new Specification<Book>(book => book.Genre == "Science fiction");
var newSpec = new Specification<Book>(book => book.PublishedAt > DateTime.UtcNow.AddDays(-7));

// You can combine rules
var featuredSpec = publishedSpec
    .And(newSpec);

var featuredBook = new Book("Dune", "Science fiction", true, DateTime.UtcNow.AddDays(-1));
var OldBook = new Book("Harry Potter", "Fantasy", true, DateTime.UtcNow.AddDays(-10));

// true
Console.WriteLine(featuredSpec.IsSatisfiedBy(featuredBook));

// false (old)
Console.WriteLine(featuredSpec.IsSatisfiedBy(OldBook));

// true
Console.WriteLine(scienceFictionSpec.IsSatisfiedBy(featuredBook));

// false (genre Fantasy)
Console.WriteLine(scienceFictionSpec.IsSatisfiedBy(OldBook));

// true (genre is not Science fiction)
Console.WriteLine(scienceFictionSpec.Not().IsSatisfiedBy(OldBook));

var featuredProjection = featuredSpec.Project<string>(book => book.Title);

// "Dune"
Console.WriteLine(featuredProjection.CompileSelector().Invoke(featuredBook));

public sealed record Book(string Title, string Genre, bool IsPublished, DateTime PublishedAt);