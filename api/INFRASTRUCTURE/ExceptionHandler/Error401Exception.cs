using System;

namespace INFRASTRUCTURE.ExceptionHandler;

public class Error401Exception : Exception
{
    public Error401Exception(string message) : base("Unauthorized: " + message)
    {
    }
}
