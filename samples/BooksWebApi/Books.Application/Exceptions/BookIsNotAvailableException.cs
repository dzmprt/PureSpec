namespace Books.Application.Exceptions;

public class BookIsNotAvailableException(int bookId) :
    ForbiddenException($"Book {bookId} is not available.");