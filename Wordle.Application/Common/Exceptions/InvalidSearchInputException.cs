using System.Net;

namespace Wordle.Application.Common.Exceptions;

public class InvalidSearchInputException : Exception
{
    public InvalidSearchInputException(string message) : base(message) { }

    public virtual HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
}
