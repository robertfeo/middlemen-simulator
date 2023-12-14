public class LoanAlreadyExistsException : Exception
{
    public LoanAlreadyExistsException()
    {
    }

    public LoanAlreadyExistsException(string message)
        : base(message)
    {
    }

    public LoanAlreadyExistsException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
