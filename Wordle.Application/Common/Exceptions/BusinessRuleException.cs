using System.Net;

namespace Wordle.Application.Common.Exceptions;

public class BusinessRuleException : Exception
{
    public BusinessRuleException(string message) : base(message) { }

    public virtual HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
}
