namespace CalendarManagement.Api.Exceptions;

/// <summary>
/// Thrown when a requested resource does not exist.
/// </summary>
public sealed class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }

    public NotFoundException(string resourceName, object key)
        : base($"{resourceName} with id '{key}' was not found.")
    {
    }
}
