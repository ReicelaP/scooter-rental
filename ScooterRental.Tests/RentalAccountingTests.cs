using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScooterRental.Interfaces;
using ScooterRental.Exceptions;

namespace ScooterRental.Tests
{
    [TestClass]
    public class RentalAccountingTests
    {
        private IRentalAccounting _accounting;
        private IList<RentedScooter> _activeRentScooterList;
        private IDatabase _database;                                
        private Dictionary<int, List<decimal>> _incomeData;      

        [TestInitialize]
        public void Setup()
        {
            _incomeData = new Dictionary<int, List<decimal>>()
            {
                { 2000, new List<decimal>(){ 2m, 4m, 5m } },
                { 2001, new List<decimal>(){ 5m } },
                { 2002, new List<decimal>(){ 2.3m, 2.8m } },
            };
                      
            _database = new Database(_incomeData);
            _accounting = new RentalAccounting(_database, _incomeData);
        }

        [TestMethod]
        public void CalculateTotalPrice_StartEndTimeTheSameDay_TotalPriceInDayLessThan20()   
        {
            //Arrange 
            var start = new DateTime(2021, 11, 10, 14, 10, 5);
            var end = new DateTime(2021, 11, 10, 14, 20, 5);
            var rentedScooter = new RentedScooter("2", start, 0.2m);
            
            //Act 
            var result = _accounting.CalculateTotalPrice(rentedScooter, start, end);

            //Assert
            result.Should().Be(2);
        }

        [TestMethod]
        public void CalculateTotalPrice_StartEndTimeTheSameDay_TotalPriceInDayMoreThan20()
        {
            //Arrange 
            var start = new DateTime(2021, 11, 10, 14, 10, 5);
            var end = new DateTime(2021, 11, 10, 14, 50, 5);
            var rentedScooter = new RentedScooter("2", start, 1m);

            //Act 
            var result = _accounting.CalculateTotalPrice(rentedScooter, start, end);

            //Assert
            result.Should().Be(20);
        }

        [TestMethod]
        public void CalculateTotalPrice_StartEndTimeNotTheSameDay_TotalPriceInDayMoreThan20()
        {
            //Arrange 
            var start = new DateTime(2021, 11, 10, 20, 0, 0);
            var end = new DateTime(2021, 11, 13, 23, 30, 0);
            var rentedScooter = new RentedScooter("2", start, 1m);

            //Act 
            var result = _accounting.CalculateTotalPrice(rentedScooter, start, end);

            //Assert
            result.Should().Be(80);
        }

        [TestMethod]
        public void CalculateTotalPrice_StartEndTimeNotTheSameDay_TotalPriceInSomeDaysLessThan20()
        {
            //Arrange 
            var start = new DateTime(2021, 11, 10, 21, 15, 0);
            var end = new DateTime(2021, 11, 13, 23, 30, 0);
            var rentedScooter = new RentedScooter("2", start, 0.1m);

            //Act 
            var result = _accounting.CalculateTotalPrice(rentedScooter, start, end);

            //Assert
            result.Should().Be(76.5m);
        }

        [TestMethod]
        public void CalculateTotalPrice_StartEndTimeNotTheSameDay_ZeroMinutes_TotalPriceInDayLessThan20()
        {
            //Arrange 
            var start = new DateTime(2021, 11, 10, 0, 0, 0);
            var end = new DateTime(2021, 11, 13, 0, 0, 0);
            var rentedScooter = new RentedScooter("2", start, 0.01m);

            //Act 
            var result = _accounting.CalculateTotalPrice(rentedScooter, start, end);

            //Assert
            result.Should().Be(43.2m);
        }

        [TestMethod]
        public void CheckIfLessThanMaxPricePerDay_PriceIsHigherThanMaxPricePerDay_ReturnMaxPricePerDay()
        {
            //Act
            var result = _accounting.CheckIfLessThanMaxPricePerDay(30, 20);

            //Assert
            result.Should().Be(20);
        }

        [TestMethod]
        public void CheckIfLessThanMaxPricePerDay_PriceIsLessThanMaxPricePerDay_ReturnPrice()
        {
            //Act
            var result = _accounting.CheckIfLessThanMaxPricePerDay(15, 20);

            //Assert
            result.Should().Be(15);
        }

        [TestMethod]
        public void GetReport_YearNoValue_GetAllIncomeSum()
        {
            //Act 
            var result = _accounting.GetReport(null);

            //Assert
            result.Should().Be(21.1m);
        }

        [TestMethod]
        public void GetReport_YearHasValue_GetIncomeSumForExpectedYear()
        {
            //Act 
            var result = _accounting.GetReport(2000);

            //Assert
            result.Should().Be(11m);
        }

        [TestMethod]
        public void GetReport_YearHasValue_YearDoesNotExist_ThrowYearDoesNotExistInDatabaseException()
        {
            // Act 
            Action act = () => _accounting.GetReport(2005);

            //Assert
            act.Should().Throw<YearDoesNotExistInDatabaseException>().WithMessage("Year 2005 is not in the database");
        }

        [TestMethod]
        public void CalculateNotCompletedRentalIncome_ActiveRentsExist_GetActiveRentIncomeSum()
        {
            //Arrange
            _activeRentScooterList = new List<RentedScooter>()
            {
                new RentedScooter("2", DateTime.Now.AddMinutes(-10), 2),
                new RentedScooter("3", DateTime.Now.AddMinutes(-7), 2),
                new RentedScooter("4", DateTime.Now.AddMinutes(-5), 2)
            };

            //Act
            var result = _accounting.CalculateNotCompletedRentalIncome(_activeRentScooterList);

            //Assert
            result.Should().Be(44m);
        }

        [TestMethod]
        public void CalculateNotCompletedRentalIncome_ActiveRentsDontExist_GetResultZero()
        {
            //Arrange
            _activeRentScooterList = new List<RentedScooter>();

            //Act
            var result = _accounting.CalculateNotCompletedRentalIncome(_activeRentScooterList);

            //Assert
            result.Should().Be(0m);
        }
    }
}
