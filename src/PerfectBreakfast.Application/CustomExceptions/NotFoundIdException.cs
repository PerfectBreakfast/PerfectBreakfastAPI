namespace PerfectBreakfast.Application.CustomExceptions;

public class NotFoundIdException : Exception
{
    public NotFoundIdException() { }
    public NotFoundIdException(string message) : base(message) { }
    public NotFoundIdException(string message, Exception inner) : base(message, inner) { }
}