using System;

namespace ScooterRental.Exceptions
{
    public class YearDoesNotExistInDatabaseException : Exception
    {
        public YearDoesNotExistInDatabaseException(int year) : 
            base($"Year {year} is not in the database") { }
    }
}
