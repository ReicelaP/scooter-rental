using System;

namespace ScooterRental.Exceptions
{
    public class ScooterIsNotRentedException : Exception
    {
        public ScooterIsNotRentedException(string id) : 
            base($"Scooter with id {id} was not rented") { }

        public ScooterIsNotRentedException() :
            base($"Selected scooter was not rented") { }
    }
}
