using System;

namespace ScooterRental.Exceptions
{
    public class ScooterAlreadyTakenException : Exception
    {
        public ScooterAlreadyTakenException(string id) : 
            base($"Scooter with id {id} is already in rent") { }
    }
}
