namespace INFRASTRUCTURE.ExceptionHandler;

public class Error500Exception : Exception
{
    public Error500Exception(string message) : base("Internal Server Error: " + message)
    {
    }
}
