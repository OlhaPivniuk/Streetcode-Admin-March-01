namespace Streetcode.BLL.Exceptions;

public abstract class NotFoundException : ApplicationException
{
    protected NotFoundException(string message)
        : base("Not Found", message)
    {
    }
}
