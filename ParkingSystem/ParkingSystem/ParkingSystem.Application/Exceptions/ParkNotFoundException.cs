namespace ParkingSystem.Application.Exceptions
{
    public class ParkNotFoundException : Exception
    {
        public ParkNotFoundException()
            : base("The provided park does not exist.")
        {
        }

        public ParkNotFoundException(string message)
            : base(message)
        {
        }

        public ParkNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
