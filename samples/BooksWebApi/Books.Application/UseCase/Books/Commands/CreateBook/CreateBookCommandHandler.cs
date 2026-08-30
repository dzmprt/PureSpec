using Books.Application.Exceptions;
using Books.Domain;
using Books.Domain.Specifications;
using Microsoft.EntityFrameworkCore;
using MitMediator;

namespace Books.Application.UseCase.Books.Commands.CreateBook;

/// <summary>
/// Handler for <see cref="CreateBookCommand"/>
/// </summary>
public class CreateBookCommandHandler(DbContext dbContext) : IRequestHandler<CreateBookCommand, Book>
{
    /// <inheritdoc/>
    /// <returns>The created book.</returns>
    public async ValueTask<Book> HandleAsync(CreateBookCommand request, CancellationToken cancellationToken)
    {
        var authorSpec = new AuthorIsDeletedSpec()
            .Not()
            .And(new AuthorByIdSpec(request.AuthorId));

        var author = await dbContext.Set<Author>().FirstOrDefaultAsync(authorSpec.Predicate, cancellationToken);
        if (author is null)
        {
            throw new BadOperationException("Author not found");
        }

        var genreSpec = new GenreByNameSpec(request.GenreName.Trim().ToUpperInvariant());

        var genre = await dbContext.Set<Genre>().FirstOrDefaultAsync(genreSpec.Predicate, cancellationToken);
        if (genre is null)
        {
            throw new BadOperationException("Genre not found");
        }

        var book = new Book(request.Title, author, genre);
        book.Publish();
        await dbContext.Set<Book>().AddAsync(book, cancellationToken);
        return book;
    }
}