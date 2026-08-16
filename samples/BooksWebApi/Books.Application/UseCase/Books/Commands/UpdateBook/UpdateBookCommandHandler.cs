using Books.Application.Exceptions;
using Books.Application.UseCase.Books.Commands.CreateBook;
using Books.Domain;
using Books.Domain.Specifications;
using MitMediator;
using PureSpec.Repositories.Abstractions;

namespace Books.Application.UseCase.Books.Commands.UpdateBook;

/// <summary>
/// Handler for <see cref="CreateBookCommand"/>
/// </summary>
public class UpdateBookCommandHandler(
    IRepository<Book> booksRepository,
    IRepository<Author> authorsRepository,
    IRepository<Genre> genresRepository) :
    IRequestHandler<UpdateBookCommand, Book>
{
    /// <inheritdoc/>
    public async ValueTask<Book> HandleAsync(UpdateBookCommand request, CancellationToken cancellationToken)
    {
        var bookSpec = new BookByIdSpec(request.BookId);

        var book = await booksRepository.FirstOrDefaultAsync(bookSpec.ToQuery(), cancellationToken);
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

        var author = await authorsRepository.FirstOrDefaultAsync(authorSpec.ToQuery(), cancellationToken);
        if (author is null)
        {
            throw new BadOperationException("Author not found");
        }

        var genreSpec = new GenreByNameSpec(request.GenreName.Trim().ToUpperInvariant());
        var genre = await genresRepository.FirstOrDefaultAsync(genreSpec.ToQuery(), cancellationToken);
        if (genre is null)
        {
            throw new BadOperationException("Genre not found");
        }

        book.SetTitle(request.Title);
        book.SetAuthor(author);
        book.SetGenre(genre);

        await booksRepository.UpdateAsync(book, cancellationToken);
        return book;
    }
}