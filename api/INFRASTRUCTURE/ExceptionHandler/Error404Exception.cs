using System;

namespace INFRASTRUCTURE.ExceptionHandler;

public class Error404Exception : Exception
{
    public Error404Exception(string message) : base("NotFound: " + message)
    {
    }
}

