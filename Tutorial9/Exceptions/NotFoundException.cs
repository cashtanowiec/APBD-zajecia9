namespace Tutorial9.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException() : base("Record has not been found in the database!")
    {
        
    }

    public NotFoundException(string? message) : base(message)
    {
        
    }
}