using PureSpec;

namespace Books.Domain.Specifications;

public class AuthorByIdSpec(int authorId) :
    Specification<Author>(author => author.AuthorId == authorId);