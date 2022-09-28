using System;
using System.Collections.Generic;

namespace ScooterRental.Interfaces
{
    public interface IRentalAccounting
    {
        decimal CalculateTotalPrice(RentedScooter rentedScooter, DateTime start, DateTime end);
        decimal GetReport(int? year);
        decimal CalculateNotCompletedRentalIncome(IList<RentedScooter> ActiveRentScooterList);
        decimal CheckIfLessThanMaxPricePerDay(decimal totalPrice, decimal maxPricePerDay);
    }
}
