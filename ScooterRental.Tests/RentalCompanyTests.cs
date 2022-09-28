using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScooterRental.Interfaces;
using ScooterRental.Exceptions;

namespace ScooterRental.Tests
{
    [TestClass]
    public class RentalCompanyTests
    {
        private IScooterService _scooterService;
        private IRentalCompany _company;
        private List<Scooter> _inventory;
        private IList<RentedScooter> _activeRentScooterList;
        private IRentalAccounting _accounting;
        private Database _database;
        private Dictionary<int, List<decimal>> _incomeData;

        [TestInitialize]
        public void Setup()
        {
            _inventory = new List<Scooter>();
            _scooterService = new ScooterService(_inventory);    
            _activeRentScooterList = new List<RentedScooter>();
            _incomeData = new Dictionary<int, List<decimal>>();
            _database = new Database(_incomeData);
            _accounting = new RentalAccounting(_database, _incomeData);
            _company = new RentalCompany("Boltxxx", _scooterService, _activeRentScooterList, _accounting, _database);
            _scooterService.AddScooter("1", 0.2m);
            _scooterService.AddScooter("2", 0.2m);
            _scooterService.AddScooter("3", 0.2m);
        }

        [TestMethod]
        public void RentalCompanyCreation_EmptyName_ThrowsInvalidNameException()
        {
            //act
            Action act = () => new RentalCompany("", _scooterService, _activeRentScooterList, _accounting, _database);

            //Assert
            act.Should().Throw<InvalidNameException>();
        }

        [TestMethod]
        public void StartRent_ScooterExists_StartRentOfScooter()
        {
            // Arrange 
            var scooter = _scooterService.GetScooterById("2");
            scooter.IsRented = false;

            // Act 
            _company.StartRent("2");

            // Assert
            scooter.IsRented.Should().BeTrue();
            _activeRentScooterList.Count.Should().Be(1);
        }

        [TestMethod]
        public void StartRent_ScooterIsRented_ThrowsScooterAlreadyTakenException()   
        {
            // Arrange 
            var scooter = _scooterService.GetScooterById("2");
            scooter.IsRented = true;

            // Act 
            Action act = () => _company.StartRent("2");

            //Assert
            act.Should().Throw<ScooterAlreadyTakenException>().WithMessage("Scooter with id 2 is already in rent");
        }

        [TestMethod]
        public void StartRent_ScooterDoesNotExist_ThrowsScooterDoesnotExistException()
        {
            //Act
            Action act = () => _company.StartRent("0");

            //Assert
            act.Should().Throw<ScooterDoesnotExistException>().WithMessage("Scooter with id 0 doesn't exist");
        }

        [TestMethod]
        public void StartRent_ScooterNullOrEmptyId_ThrowsInvalidIdException()
        {
            //Act
            Action act = () => _company.StartRent("");

            //Assert
            act.Should().Throw<InvalidIdException>().WithMessage("Id cannot be null or empty");
        }

        [TestMethod]
        public void EndRent_ScooterWasRented_EndRentOfScooter()
        {
            // Arrange 
            var scooter = _scooterService.GetScooterById("2");
            var rentedScooter = new RentedScooter("2", DateTime.Now.AddMinutes(-10), 0.2m);
            _activeRentScooterList.Add(rentedScooter);
            scooter.IsRented = true;
           
            // Act 
            var result = _company.EndRent("2");

            // Assert
            scooter.IsRented.Should().BeFalse();
            rentedScooter.EndTime.HasValue.Should().BeTrue();
            _activeRentScooterList.Count.Should().Be(0);         
            result.Should().Be(2);
        }

        [TestMethod]
        public void EndRent_ScooterWasNotRented_ThrowScooterIsNotRentedException() 
        {
            // Arrange 
            var scooter = _scooterService.GetScooterById("2");
            var rentedScooter = new RentedScooter("2", DateTime.Now.AddMinutes(-10), 0.2m);
            scooter.IsRented = false;

            // Act 
            Action act = () => _company.EndRent("2");

            // Assert
            act.Should().Throw<ScooterIsNotRentedException>().WithMessage("Scooter with id 2 was not rented");
        }

        [TestMethod]
        public void EndRent_ScooterDoesNotExist_ThrowsScooterDoesnotExistException()
        {
            //Act
            Action act = () => _company.StartRent("0");

            //Assert
            act.Should().Throw<ScooterDoesnotExistException>().WithMessage("Scooter with id 0 doesn't exist");
        }

        [TestMethod]
        public void EndRent_ScooterNullOrEmptyId_ThrowsInvalidIdException()
        {
            //Act
            Action act = () => _company.StartRent("");

            //Assert
            act.Should().Throw<InvalidIdException>().WithMessage("Id cannot be null or empty");
        }

        [TestMethod]
        public void CalculateIncome_YearNoValue_OnlyCompletedRentals_GetAllIncome()
        {
            //Arrange
            _database.AddToDatabase(2000, 2m);
            _database.AddToDatabase(2001, 5m);
            _database.AddToDatabase(2003, 2.8m);
            _activeRentScooterList.Add(new RentedScooter("2", DateTime.Now.AddMinutes(-10), 2));
            _activeRentScooterList.Add(new RentedScooter("3", DateTime.Now.AddMinutes(-7), 2));

            //Act 
            var result = _company.CalculateIncome(null, false);

            //Assert
            result.Should().Be(9.8m);
        }

        [TestMethod]
        public void CalculateIncome_YearNoValue_IncludeNotCompletedRentals_GetAllIncome()
        {
            //Arrange
            _database.AddToDatabase(2000, 2m);
            _database.AddToDatabase(2001, 5m);
            _database.AddToDatabase(2003, 2.8m);
            _activeRentScooterList.Add(new RentedScooter("2", DateTime.Now.AddMinutes(-10), 2));
            _activeRentScooterList.Add(new RentedScooter("3", DateTime.Now.AddMinutes(-7), 2));

            //Act 
            var result = _company.CalculateIncome(null, true);

            //Assert
            result.Should().Be(43.8m);
        }

        [TestMethod]
        public void CalculateIncome_YearHasValue_OnlyCompletedRentals_GetIncomeForExpectedYear()
        {
            //Arrange
            _database.AddToDatabase(2000, 2m);
            _database.AddToDatabase(2001, 5m);
            _database.AddToDatabase(2003, 2.8m);
            _activeRentScooterList.Add(new RentedScooter("2", DateTime.Now.AddMinutes(-10), 2));
            _activeRentScooterList.Add(new RentedScooter("3", DateTime.Now.AddMinutes(-7), 2));

            //Act 
            var result = _company.CalculateIncome(2001, false);

            //Assert
            result.Should().Be(5m);
        }

        [TestMethod]
        public void CalculateIncome_YearHasValue_IncludeNotCompletedRentals_GetAllIncome()
        {
            //Arrange
            _database.AddToDatabase(2000, 2m);
            _database.AddToDatabase(2001, 5m);
            _database.AddToDatabase(2003, 2.8m);
            _activeRentScooterList.Add(new RentedScooter("2", DateTime.Now.AddMinutes(-10), 2));
            _activeRentScooterList.Add(new RentedScooter("3", DateTime.Now.AddMinutes(-7), 2));

            //Act 
            var result = _company.CalculateIncome(2001, true);

            //Assert
            result.Should().Be(5m);
        }
    }  
}
