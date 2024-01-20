namespace PerfectBreakfast.Application.CustomExceptions;

public class IdentityException : Exception
{
    public IdentityException() { }
    public IdentityException(string message) : base(message) { }
    public IdentityException(string message, Exception inner) : base(message, inner) { }
}