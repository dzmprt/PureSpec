using System.Linq.Expressions;
using PureSpec;

namespace Books.Domain.Specifications;

public class BookByIdSpec(int bookId) :
    Specification<Book>(book => book.BookId == bookId);