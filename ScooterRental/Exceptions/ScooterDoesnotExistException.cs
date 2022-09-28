using System;

namespace ScooterRental.Exceptions
{
    public class ScooterDoesnotExistException : Exception
    {
        public ScooterDoesnotExistException(string id) : 
            base($"Scooter with id {id} doesn't exist") { }

        public ScooterDoesnotExistException() :
            base($"Scooter doesn't exist")
        { }
    }
}
