using Books.Application.Exceptions;
using Books.Application.UseCase.Books.Commands.CreateBook;
using Books.Domain;
using Books.Domain.Specifications;
using Microsoft.EntityFrameworkCore;
using MitMediator;

namespace Books.Application.UseCase.Books.Commands.UpdateBook;

/// <summary>
/// Handler for <see cref="CreateBookCommand"/>
/// </summary>
public class UpdateBookCommandHandler(DbContext dbContext) :
    IRequestHandler<UpdateBookCommand, Book>
{
    /// <inheritdoc/>
    public async ValueTask<Book> HandleAsync(UpdateBookCommand request, CancellationToken cancellationToken)
    {
        var bookSpec = new BookByIdSpec(request.BookId);

        var book = await dbContext.Set<Book>().FirstOrDefaultAsync(bookSpec.Predicate, cancellationToken);
        if (book is null)
        {
            throw new NotFoundException();
        }

        if (!new BookIsAvailableSpec().IsSatisfiedBy(book))
        {
            throw new BookIsNotAvailableException(request.BookId);
        }

        var authorSpec = new AuthorByIdSpec(request.AuthorId)
            .AndNot(new AuthorIsDeletedSpec());

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

        book.SetTitle(request.Title);
        book.SetAuthor(author);
        book.SetGenre(genre);
        return book;
    }
}