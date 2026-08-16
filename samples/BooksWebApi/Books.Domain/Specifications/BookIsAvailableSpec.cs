using PureSpec;

namespace Books.Domain.Specifications;

public class BookIsAvailableSpec() :
    Specification<Book>(book =>
        book.IsPublished &&
        !book.Author.IsDeleted);