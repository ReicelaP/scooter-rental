using System;
using System.Linq;
using System.Collections.Generic;
using ScooterRental.Interfaces;
using ScooterRental.Exceptions;

namespace ScooterRental
{
    public class RentalCompany : IRentalCompany
    {
        public string Name { get; }
        private IList<RentedScooter> _activeRentScooterList;
        private IScooterService _scooterService;
        private IRentalAccounting _accounting;
        private IDatabase _database;

        public RentalCompany (string name, 
            IScooterService scooterService, 
            IList<RentedScooter> rentedScooterList, 
            IRentalAccounting accounting, 
            IDatabase database)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new InvalidNameException();
            }

            Name = name;
            _scooterService = scooterService;
            _activeRentScooterList = rentedScooterList;
            _accounting = accounting;
            _database = database;
        }

        public void StartRent(string id)
        {
            var scooter = _scooterService.GetScooterById(id);

            if(scooter.IsRented == false)
            {
                scooter.IsRented = true;
                _activeRentScooterList.Add(new RentedScooter(scooter.Id, DateTime.Now, scooter.PricePerMinute));
            }
            else
            {
                throw new ScooterAlreadyTakenException(id);
            }
        }

        public decimal EndRent(string id)
        {
            var scooter = _scooterService.GetScooterById(id);
            var rentedScooter = _activeRentScooterList.FirstOrDefault(s => s.Id == id && !s.EndTime.HasValue);

            if (scooter.IsRented == true)
            {
                rentedScooter.EndTime = DateTime.Now;
                scooter.IsRented = false;
                _activeRentScooterList.Remove(rentedScooter);

                var rentTotalPrice = _accounting.CalculateTotalPrice(rentedScooter, rentedScooter.StartTime, (DateTime)rentedScooter.EndTime);
                _database.AddToDatabase(((DateTime)(rentedScooter.EndTime)).Year, rentTotalPrice);

                return rentTotalPrice;
            }
            else
            {
                throw new ScooterIsNotRentedException(id);
            }
        }

        public decimal CalculateIncome(int? year, bool includeNotCompletedRentals)
        {
            var yearNotProvided = !year.HasValue;

            if (includeNotCompletedRentals && yearNotProvided)
            {
                return _accounting.GetReport(year) + _accounting.CalculateNotCompletedRentalIncome(_activeRentScooterList);
            }
            else
            {
                return _accounting.GetReport(year);
            }
        }
    }
}
