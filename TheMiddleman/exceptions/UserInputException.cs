public class UserInputException : Exception
{
    public UserInputException()
    {
    }

    public UserInputException(string message)
        : base(message)
    {
    }

    public UserInputException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
