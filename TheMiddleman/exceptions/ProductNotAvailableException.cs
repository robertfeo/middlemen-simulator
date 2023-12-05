using System;

public class ProductNotAvailableException : Exception
{
    public ProductNotAvailableException()
    {
    }

    public ProductNotAvailableException(string message)
        : base(message)
    {
    }

    public ProductNotAvailableException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
