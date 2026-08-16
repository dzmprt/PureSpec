using System.Linq.Expressions;
using PureSpec;

namespace Books.Domain.Specifications;

public class GenreByNameSpec(string genreName) :
    Specification<Genre>(genre => genre.GenreName == genreName);