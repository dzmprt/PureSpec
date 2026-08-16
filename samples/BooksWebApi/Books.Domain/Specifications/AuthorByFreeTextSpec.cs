using PureSpec;

namespace Books.Domain.Specifications;

public class AuthorByFreeTextSpec(string freeText) :
    Specification<Author>(author => author.FirstName.Contains(freeText) || author.LastName.Contains(freeText));