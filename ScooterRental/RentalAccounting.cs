using System;
using System.Linq;
using System.Collections.Generic;
using ScooterRental.Exceptions;
using ScooterRental.Interfaces;

namespace ScooterRental
{
    public class RentalAccounting : IRentalAccounting
    {
        private IDatabase _database;
        private Dictionary<int, List<decimal>> _incomeData;
        private const int MINUTES_IN_DAY = 1440;
        private const decimal MAX_PRICE_PER_DAY = 20;

        public RentalAccounting(IDatabase database, Dictionary<int, List<decimal>> incomeData)
        {
            _database = database;
            _incomeData = incomeData;
        }

        public decimal CalculateTotalPrice(RentedScooter rentedScooter, DateTime start, DateTime end)
        {
            if(start.Date == end.Date)
            {
                var minutes = (end - start).TotalMinutes;
                var totalPrice = (decimal)minutes * rentedScooter.PricePerMinute;
                var result = CheckIfLessThanMaxPricePerDay(totalPrice, MAX_PRICE_PER_DAY);
                return Math.Round(result, 2);
            }
            else
            {
                var totalPriceFirstDay = (decimal)(start.Date.AddDays(1) - start).TotalMinutes * rentedScooter.PricePerMinute;
                var totalPriceLastDay = (decimal)(end - end.Date).TotalMinutes * rentedScooter.PricePerMinute;
                var totalPriceFor24H = MINUTES_IN_DAY * rentedScooter.PricePerMinute;

                var finalPriceFirstDay = CheckIfLessThanMaxPricePerDay(totalPriceFirstDay, MAX_PRICE_PER_DAY);
                var finalPriceLastDay = CheckIfLessThanMaxPricePerDay(totalPriceLastDay, MAX_PRICE_PER_DAY);
                var finalPriceFor24H = CheckIfLessThanMaxPricePerDay(totalPriceFor24H, MAX_PRICE_PER_DAY);

                var daysInMiddle = (decimal)(end.Date - start.Date).TotalDays - 1;
                var result = finalPriceFirstDay + finalPriceLastDay + finalPriceFor24H * daysInMiddle;
                return Math.Round(result, 2);
            }
        }

        public decimal CheckIfLessThanMaxPricePerDay(decimal totalPrice, decimal maxPricePerDay)
        {
            return totalPrice < maxPricePerDay ? totalPrice : maxPricePerDay;
        }

        public decimal GetReport(int? year)
        {
            var yearNotProvided = !year.HasValue;

            if (yearNotProvided)
            {
                decimal totalSum = 0;

                foreach (var item in _incomeData)
                {
                    totalSum += item.Value.Sum();
                }
                
                return totalSum;
            }
            else
            {
                if (_incomeData.ContainsKey((int)year))
                {
                    var rentTotalPriceList = _incomeData[(int)year];
                    return rentTotalPriceList.Sum();
                }
                else
                {
                    throw new YearDoesNotExistInDatabaseException((int)year);
                }
            }
        }

        public decimal CalculateNotCompletedRentalIncome(IList<RentedScooter> ActiveRentScooterList)
        {
            decimal notCompletedRentalIncome = 0;

            foreach (var scooter in ActiveRentScooterList)
            {
                notCompletedRentalIncome += CalculateTotalPrice(scooter, scooter.StartTime, DateTime.Now);
            }

            return notCompletedRentalIncome;
        }
    }
}
