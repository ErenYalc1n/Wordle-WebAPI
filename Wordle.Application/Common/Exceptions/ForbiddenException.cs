using System.Net;

namespace Wordle.Application.Common.Exceptions;

public class ForbiddenException : Exception
{
    public ForbiddenException(string message) : base(message) { }

    public virtual HttpStatusCode StatusCode => HttpStatusCode.Forbidden;
}
