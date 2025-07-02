using System.Net;

namespace Wordle.Application.Common.Exceptions;

public class UnauthorizedAppException : Exception
{
    public UnauthorizedAppException(string message) : base(message) { }

    public virtual HttpStatusCode StatusCode => HttpStatusCode.Unauthorized;
}
