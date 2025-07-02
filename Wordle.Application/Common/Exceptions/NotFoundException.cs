using System.Net;

namespace Wordle.Application.Common.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }

    public virtual HttpStatusCode StatusCode => HttpStatusCode.NotFound;
}
