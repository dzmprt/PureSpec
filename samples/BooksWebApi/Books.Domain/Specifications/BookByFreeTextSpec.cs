using System.Linq.Expressions;
using PureSpec;

namespace Books.Domain.Specifications;

public class BookByFreeTextSpec(string freeText) :
    Specification<Book>(
            book => book.Author.FirstName.Contains(freeText) ||
                    book.Author.LastName.Contains(freeText) ||
                    book.Title.Contains(freeText) ||
                    book.Genre.GenreName.Contains(freeText));