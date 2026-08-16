using PureSpec;

namespace Books.Domain.Specifications;

public class AuthorIsDeletedSpec() :
    Specification<Author>(author => author.IsDeleted);