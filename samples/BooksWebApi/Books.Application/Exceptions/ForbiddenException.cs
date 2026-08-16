namespace Books.Application.Exceptions;

public class ForbiddenException(string? message) :
    Exception(string.IsNullOrWhiteSpace(message) ? "Forbidden" : message);