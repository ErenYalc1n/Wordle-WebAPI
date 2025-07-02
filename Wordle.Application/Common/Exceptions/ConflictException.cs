using System.Net;

namespace Wordle.Application.Common.Exceptions;

public class ConflictException : Exception
{
    public ConflictException(string message) : base(message) { }

    public virtual HttpStatusCode StatusCode => HttpStatusCode.Conflict;
}
