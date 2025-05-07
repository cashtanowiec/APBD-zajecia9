namespace Tutorial9.Exceptions;

public class OrderFulfilledException : Exception
{
    public OrderFulfilledException() : base("Order is already fulfilled!")
    {
    }

    public OrderFulfilledException(string? message) : base(message)
    {
    }
}