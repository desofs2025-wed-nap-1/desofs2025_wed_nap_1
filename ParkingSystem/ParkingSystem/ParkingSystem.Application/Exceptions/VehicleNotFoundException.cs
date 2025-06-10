namespace ParkingSystem.Application.Exceptions
{
    public class VehicleNotFoundException : Exception
    {
        public VehicleNotFoundException()
            : base("The provided vehicle does not exist.")
        {
        }

        public VehicleNotFoundException(string message)
            : base(message)
        {
        }

        public VehicleNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
