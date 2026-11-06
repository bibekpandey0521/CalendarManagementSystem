namespace CalendarManagement.Api.Exceptions;

/// <summary>
/// Thrown when a request passes model binding but breaks a business rule.
/// </summary>
public sealed class DomainValidationException : Exception
{
    public DomainValidationException(string message) : base(message)
    {
    }

    public DomainValidationException(IReadOnlyDictionary<string, string[]> errors)
        : base("One or more business rules were violated.")
    {
        Errors = errors;
    }

    public IReadOnlyDictionary<string, string[]>? Errors { get; }
}
