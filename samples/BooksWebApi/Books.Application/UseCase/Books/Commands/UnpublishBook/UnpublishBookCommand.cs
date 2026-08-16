using MitMediator;

namespace Books.Application.UseCase.Books.Commands.UnpublishBook;

/// <summary>
/// Delete book command.
/// </summary>
public struct UnpublishBookCommand : IRequest
{
    /// <summary>
    /// Book id.
    /// </summary>
    public int BookId { get; init; }
}