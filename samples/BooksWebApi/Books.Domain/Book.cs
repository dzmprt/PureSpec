using Books.Domain.Specifications;

namespace Books.Domain;

/// <summary>
/// Book.
/// </summary>
public class Book
{
    /// <summary>
    /// Max title length.
    /// </summary>
    public const int MaxTitleLength = 1000;

    /// <summary>
    /// Book id.
    /// </summary>
    public int BookId { get; private set; }

    /// <summary>
    /// Title.
    /// </summary>
    public string Title { get; private set; } = null!;

    /// <summary>
    /// Author.
    /// </summary>
    public Author Author { get; private set; } = null!;

    /// <summary>
    /// Genre.
    /// </summary>
    public Genre Genre { get; private set; } = null!;

    /// <summary>
    /// Is book published.
    /// </summary>
    public bool IsPublished { get; private set; }

    private Book() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Book"/>.
    /// </summary>
    /// <param name="title">Title.</param>
    /// <param name="author">Author.</param>
    /// <param name="genre">Genre.</param>
    /// <exception cref="ArgumentException">Incorrect title.</exception>
    public Book(string title, Author author, Genre genre)
    {
        ArgumentNullException.ThrowIfNull(author);
        ArgumentNullException.ThrowIfNull(genre);

        if (new AuthorIsDeletedSpec().IsSatisfiedBy(author))
        {
            throw new ArgumentException($"Author {author.AuthorId} is deleted.", nameof(author));
        }
        SetTitle(title);
        SetAuthor(author);
        SetGenre(genre);
    }

    /// <summary>
    /// Set genre.
    /// </summary>
    /// <param name="genre">Genre.</param>
    public void SetGenre(Genre genre)
    {
        ArgumentNullException.ThrowIfNull(genre);
        Genre = genre;
    }

    /// <summary>
    /// Set author.
    /// </summary>
    /// <param name="author">Author.</param>
    public void SetAuthor(Author author)
    {
        ArgumentNullException.ThrowIfNull(author);

        if (new AuthorIsDeletedSpec().IsSatisfiedBy(author))
        {
            throw new ArgumentException($"Author {author.AuthorId} is deleted.", nameof(author));
        }
        Author = author;
    }

    public void Publish()
    {
        IsPublished = true;
    }

    public void Hide()
    {
        IsPublished = false;
    }

    /// <summary>
    /// Set title.
    /// </summary>
    /// <param name="title">Title.</param>
    /// <exception cref="ArgumentException">Incorrect title.</exception>
    public void SetTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException($"{nameof(title)} is empty.", nameof(title));
        }

        if (title.Length > MaxTitleLength)
        {
            throw new ArgumentException($"{nameof(title)} cannot exceed {MaxTitleLength} characters.", nameof(title));
        }
        Title = title.Trim().ToUpperInvariant();
    }

    public void Unpublish()
    {
        IsPublished = false;
    }
}