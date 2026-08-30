# PureSpec

[![Build and Test](https://github.com/dzmprt/PureSpec/actions/workflows/dotnet.yml/badge.svg)](https://github.com/dzmprt/PureSpec/actions/workflows/dotnet.yml)
![NuGet](https://img.shields.io/nuget/v/PureSpec)
![.NET 10](https://img.shields.io/badge/Version-.NET%2010-informational?style=flat&logo=dotnet)
![License](https://img.shields.io/github/license/dzmprt/PureSpec)

PureSpec is a minimalist implementation of the Specification pattern for creating reusable business rules.

## Features

- Expression-based specifications.
- Rule composition with `And`, `Or`, `AndNot`, `OrNot`, and `Not`.
- Rule checks.
- Reusable projections.
- Expressions compatible with LINQ and Entity Framework Core.

## Getting Started

### Installation

```bash
dotnet add package PureSpec --version 0.0.1-alpha-7
```

**Create a rule from an expression:**

```csharp
using PureSpec;

var publishedSpec = new Specification<Book>(book => book.IsPublished);
var scienceFictionSpec = new Specification<Book>(book => book.Genre == "Science fiction");
var newSpec = new Specification<Book>(book => book.PublishedAt > DateTime.UtcNow.AddDays(-7));

// You can combine rules
var featuredSpec = publishedSpec
    .And(newSpec);

var featuredBook = new Book("Dune", "Science fiction", true, DateTime.UtcNow.AddDays(-1));
var oldBook = new Book("Harry Potter", "Fantasy", true, DateTime.UtcNow.AddDays(-10));

// true
Console.WriteLine(featuredSpec.IsSatisfiedBy(featuredBook));

// false (old)
Console.WriteLine(featuredSpec.IsSatisfiedBy(oldBook));

// true
Console.WriteLine(scienceFictionSpec.IsSatisfiedBy(featuredBook));

// false (genre Fantasy)
Console.WriteLine(scienceFictionSpec.IsSatisfiedBy(oldBook));

// true (genre is not Science fiction)
Console.WriteLine(scienceFictionSpec.Not().IsSatisfiedBy(oldBook));

public sealed record Book(string Title, string Genre, bool IsPublished, DateTime PublishedAt);
```

**Create specification type for reuse:**

```csharp
using PureSpec;

var deletedAuthor = new Author("Clara Jennings", true);
var availableAuthor = new Author("Thomas McRae", false);

var books = new List<Book>
{
    new Book("The Vanishing Hour", true, deletedAuthor),
    new Book("Ashes Beneath", false, deletedAuthor),
    new Book("Dead Man's Loop", true, availableAuthor),
    new Book("Code Silence", false, availableAuthor)
};

var bookIsAvailableSpec = new BookIsAvailableSpec();

var availableBooks = books.Where(bookIsAvailableSpec.ToFunc()).ToList();

// Only "Dead Man's Loop" is published and has an author who is not deleted.
foreach (var availableBook in availableBooks)
{
    Console.WriteLine(availableBook.Title);
}

public class BookIsAvailableSpec() :
    Specification<Book>(book =>
        book.IsPublished &&
        !book.Author.IsDeleted);

public sealed record Book(string Title, bool IsPublished, Author Author);

public sealed record Author(string Name, bool IsDeleted);
```

**Project a specification:**

```csharp
var projection = bookIsAvailableSpec.Project<BookDto>(
    book => new BookDto(book.Title));

var dto = projection.CompileSelector()(books[2]);

public sealed record BookDto(string Title);
```

**or**

```csharp
var projection = new SpecificationProjection<Book, BookDto>(
    bookIsAvailableSpec,
    book => new BookDto(book.Title));

public sealed record BookDto(string Title);
```

**or**

```csharp
var projection = new BookProjection();

public class BookProjection() :
    SpecificationProjection<Book, BookDto>(
        new BookIsAvailableSpec(),
        book => new BookDto(book.Title));

public sealed record BookDto(string Title);
```

A specification projection exposes both `Predicate` and `Selector` expressions. Use `CompilePredicate` and `CompileSelector` for in-memory evaluation, or pass the expressions directly to LINQ providers.

### LINQ queries

Use `Predicate` with `IQueryable<TEntity>` and compose sorting and paging with standard LINQ operators:

```csharp
var specification = new BookIsAvailableSpec();

var books = await dbContext.Books
    .AsNoTracking()
    .Where(specification.Predicate)
    .OrderBy(book => book.Title)
    .Skip(10)
    .Take(20)
    .ToArrayAsync(cancellationToken);
```

The expression remains available to the query provider, so Entity Framework Core can translate it to SQL. For in-memory collections, use `ToFunc()` or compile the predicate:

```csharp
var books = allBooks.Where(specification.ToFunc()).ToArray();
```

Apply a specification projection by using both expressions:

```csharp
var specification = new BookIsAvailableSpec().Project(
    book => new BookDto(book.Title));

var books = await dbContext.Books
    .AsNoTracking()
    .Where(specification.Predicate)
    .Select(specification.Selector)
    .ToArrayAsync(cancellationToken);
```

PureSpec does not provide repository, persistence, or transaction abstractions. Use the data access API appropriate for your application for writes and transaction management.

## License

[MIT](LICENSE)
