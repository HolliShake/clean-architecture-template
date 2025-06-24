namespace INFRASTRUCTURE.ExceptionHandler;

public class Error400Exception : Exception
{
    public Error400Exception(string message) : base("BadRequest: " + message)
    {
    }
}
