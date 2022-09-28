using System.Linq;
using System.Collections.Generic;
using ScooterRental.Interfaces;
using ScooterRental.Exceptions;
using ScooterRental.Validators;

namespace ScooterRental
{
    public class ScooterService : IScooterService
    {
        private readonly List<Scooter> _scooters;

        public ScooterService(List<Scooter> inventory)
        {
            _scooters = inventory;
        }

        public void AddScooter(string id, decimal pricePerMinute)
        {
            Validator.ScooterIdValidator(id);

            if (pricePerMinute <= 0)
            {
                throw new InvalidPriceException(pricePerMinute);
            }

            if(_scooters.Any(scooter => scooter.Id == id))
            {
                throw new DuplicateScooterException(id);
            }

            _scooters.Add(new Scooter(id, pricePerMinute));
        }

        public void RemoveScooter(string id)
        {
            Validator.ScooterIdValidator(id);

            var scooter = _scooters.FirstOrDefault(scooter => scooter.Id == id);

            if(scooter == null)
            {
                throw new ScooterDoesnotExistException(id);
            }
            
            _scooters.Remove(scooter);
        }

        public IList<Scooter> GetScooters()
        {
            return _scooters.ToList();
        }

        public Scooter GetScooterById(string scooterId)
        {
            Validator.ScooterIdValidator(scooterId);

            var scooter = _scooters.FirstOrDefault(scooter => scooter.Id == scooterId);

            if (scooter == null)
            {
                throw new ScooterDoesnotExistException(scooterId);
            }

            return scooter;
        }
    }
}
