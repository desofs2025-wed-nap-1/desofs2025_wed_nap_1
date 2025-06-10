namespace ParkingSystem.Application.Exceptions
{
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException()
            : base("The provided user does not exist.")
        {
        }

        public UserNotFoundException(string message)
            : base(message)
        {
        }

        public UserNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
