# PureSpec

[![Build and Test](https://github.com/dzmprt/PureSpec/actions/workflows/dotnet.yml/badge.svg)](https://github.com/dzmprt/PureSpec/actions/workflows/dotnet.yml)
![NuGet](https://img.shields.io/nuget/v/PureSpec)
![.NET 10](https://img.shields.io/badge/Version-.NET%2010-informational?style=flat&logo=dotnet)
![License](https://img.shields.io/github/license/dzmprt/PureSpec)

PureSpec is a minimalist implementation of the Specification pattern for creating reusable business rules and database queries.

## Features

- Expression-based specifications.
- Rule composition with `And`, `Or`, `AndNot`, `OrNot`, and `Not`.
- Rule checks.
- Projections for specs and queries.
- Query sorting and paging.
- Repository interfaces that do not depend on Entity Framework Core.
- Optional Entity Framework Core provider and repository.
- Async read, write, and transaction operations.

## Getting Started

### Installation

```bash
# Specifications
dotnet add package PureSpec --version 0.0.1-alpha

# Repository, provider, transaction manager interfaces
dotnet add package PureSpec.Repositories.Abstractions --version 0.0.1-alpha

# Entity Framework Core implementation
dotnet add package PureSpec.Repositories.EntityFrameworkCore --version 0.0.1-alpha
```

### PureSpec

The `PureSpec` package contains only the core specification API.

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
var projectedSpec = bookIsAvailableSpec.Project<BookDto>(
    book => new BookDto(book.Title));

var dto = projectedSpec.CompileSelector()(books[2]);

public sealed record BookDto(string Title);
```

**or**

```csharp
var projectedSpec = new ProjectedSpecification<Book, BookDto>(
    bookIsAvailableSpec,
    book => new BookDto(book.Title));

public sealed record BookDto(string Title);
```

**or**

```csharp
var projectedSpec = new BookProjectionSpec();

public class BookProjectionSpec() :
    ProjectedSpecification<Book, BookDto>(
        new BookIsAvailableSpec(),
        book => new BookDto(book.Title));

public sealed record BookDto(string Title);
```

A projected specification exposes both `Predicate` and `Selector` expressions. Use `CompilePredicate` and `CompileSelector` for in-memory evaluation, or pass the expressions to a projected repository query as shown below.

### Repository abstractions

The `PureSpec.Repositories.Abstractions` package defines query and repository contracts.

**Use `ToQuery` extension methods to create `IQuery`:**

```csharp
using PureSpec.Repositories.Abstractions;

// Convert a specification to a filtered query.
var query = featuredSpec.ToQuery();

// Sort by PublishedAt descending, skip 10 results, and return at most 5.
var last5BooksQuery = featuredSpec.ToQuery(book => book.PublishedAt, descending: true, limit: 5, offset: 10);
```

Every query exposes a `Predicate`, `Limit`, `Offset`, and `Orderings`. A projected query also exposes a `Selector`. Use `query.ApplyQuery(queryable)` to apply filtering, ordering, paging, and projection to an `IQueryable<TEntity>` source. `limit` must be greater than zero, and `offset` cannot be negative.

Create a projected query from a projected specification:

```csharp
var projectedSpec = bookIsAvailableSpec.Project(
    book => new BookDto(book.Title));

// Project, sort by title, and return at most 20 results.
var projectedQuery = projectedSpec.ToQuery(
    book => book.Title,
    limit: 20);
```

You can implement `IProvider<TEntity>` and `IRepository<TEntity>` yourself, or use the implementations from `PureSpec.Repositories.EntityFrameworkCore`.

Interfaces for implementation:

- `IProvider<TEntity>` for read operations;
- `IRepository<TEntity>` for read and write operations;
- `ITransactionManager` for transaction control.

The package provides `Query<TEntity>` and `Query<TEntity, TResult>` implementations for filtered queries and `QueryOrder<TEntity, TKey>` for sorting. Custom implementations are only necessary when these types do not cover a provider's requirements.

### Entity Framework Core implementation

The `PureSpec.Repositories.EntityFrameworkCore` package provides implementations:

- `BaseProvider<TEntity>` for asynchronous reads;
- `BaseRepository<TEntity>` for reads and writes;
- `TransactionManager` for database transactions.

Register the services with dependency injection:

```csharp
using PureSpec.Repositories.EntityFrameworkCore;

services.AddPureSpecRepositories();
```

Register your `DbContext` before resolving these services. The extension registers `IProvider<TEntity>` and `IRepository<TEntity>` as transient services and `ITransactionManager` as scoped.

Use the repository in an application service:

```csharp
using PureSpec;
using PureSpec.Repositories.Abstractions;

public sealed class BookService(IProvider<Book> booksProvider)
{
    public ValueTask<Book[]> GetBooksAsync(
        int? limit,
        int? offset,
        CancellationToken cancellationToken)
    {
        var specification = new BookIsAvailableSpec();

        return booksProvider.ToArrayAsync(
            specification.ToQuery(book => book.Title, limit, offset),
            cancellationToken);
    }
}

public class BookIsAvailableSpec() :
    Specification<Book>(book =>
        book.IsPublished &&
        !book.Author.IsDeleted);

public sealed record Book(string Title, bool IsPublished, Author Author);

public sealed record Author(string Name, bool IsDeleted);
```

`BaseProvider<TEntity>` uses no-tracking queries by default. `BaseRepository<TEntity>` uses tracking because it also supports add, update, and delete operations.

#### Repository operations

`IProvider<TEntity>` supports:

- `ToArrayAsync`;
- `FirstOrDefaultAsync`;
- `SingleOrDefaultAsync`;
- `SingleAsync`;
- `CountAsync`;
- `AnyAsync`.

Entity and projected query overloads are available where applicable. Operations other than `CountAsync` also have overloads that do not require a query.

`IRepository<TEntity>` also supports:

- `AddAsync`;
- `UpdateAsync`;
- `DeleteAsync`.

`DeleteAsync` requires the query to match exactly one entity. It throws `EntityNotFoundException` when no entity is found and follows `SingleOrDefaultAsync` semantics when multiple entities match.

#### Transactions

Use `ITransactionManager` to control a transaction:

```csharp
await transactionManager.BeginTransactionAsync(cancellationToken);

// Make repository changes here.

await transactionManager.CommitTransactionAsync(cancellationToken);
```

Call `RollbackTransactionAsync` when the operation must be undone. Commit and rollback are no-ops when there is no active transaction.

> [!WARNING]
> `BaseRepository<TEntity>` calls `SaveChangesAsync` and starts a transaction through the transaction manager when `AddAsync`, `UpdateAsync`, or `DeleteAsync` is called and no transaction is active. It does not commit that transaction. Call `CommitTransactionAsync` after all related writes, or call `RollbackTransactionAsync` when the operation fails.

## License

[MIT](LICENSE)
