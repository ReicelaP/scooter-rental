using ScooterRental.Exceptions;

namespace ScooterRental.Validators
{
    public class Validator
    {
        public static void ScooterIdValidator(string id)
        {
            if (string.IsNullOrEmpty(id))        
            {
                throw new InvalidIdException();
            }
        }
    }
}
