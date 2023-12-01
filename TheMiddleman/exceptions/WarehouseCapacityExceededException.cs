using System;

public class WarehouseCapacityExceededException : Exception
{
    public WarehouseCapacityExceededException()
    {
    }

    public WarehouseCapacityExceededException(string message)
        : base(message)
    {
    }

    public WarehouseCapacityExceededException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
