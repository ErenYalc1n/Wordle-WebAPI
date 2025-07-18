﻿using System.Net;

namespace Wordle.Application.Common.Exceptions;

public class ValidationAppException : Exception
{
    public ValidationAppException(string message) : base(message) { }

    public virtual HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
}
