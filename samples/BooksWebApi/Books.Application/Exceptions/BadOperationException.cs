namespace Books.Application.Exceptions;

/// <summary>
/// Bad operation exception.
/// </summary>
public class BadOperationException(string? message) :
    Exception(message);