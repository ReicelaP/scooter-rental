using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;
using ScooterRental.Exceptions;
using ScooterRental.Interfaces;

namespace ScooterRental.Tests
{
    [TestClass]
    public class RentalCompanyMoqTests
    {
        private IRentalCompany _company;
        private AutoMocker _mocker;
        private Mock<IScooterService> _scooterServiceMock;
        private IList<RentedScooter> _activeRentScooterList;
        private Mock<IRentalAccounting> _accountingMock;
        private Mock<IDatabase> _databaseMock;
        private Scooter _defaultScooter;

        [TestInitialize]
        public void SetUp()
        {
            _defaultScooter = new Scooter("1", 0.2m);
            _mocker = new AutoMocker();
            _scooterServiceMock = _mocker.GetMock<IScooterService>();
            _activeRentScooterList = new List<RentedScooter>();
            _accountingMock = _mocker.GetMock<IRentalAccounting>();
            _databaseMock = _mocker.GetMock<IDatabase>();
            _company = new RentalCompany("Boltx", _scooterServiceMock.Object, _activeRentScooterList, _accountingMock.Object, _databaseMock.Object);
        }

        [TestMethod]
        public void StartRent_ScooterExists_StartRentOfScooter()
        {
            //Arrange
            _scooterServiceMock.Setup(s => s.GetScooterById("1")).Returns(_defaultScooter);

            //Act
            _company.StartRent("1");

            //Asseet
            _defaultScooter.IsRented.Should().BeTrue();
        }

        [TestMethod]
        public void StartRent_ScooterIsRented_ThrowsScooterAlreadyTakenException()
        {
            // Arrange 
            _scooterServiceMock.Setup(s => s.GetScooterById("1")).Returns(_defaultScooter);
            _defaultScooter.IsRented = true;

            // Act 
            Action act = () => _company.StartRent("1");

            //Assert
            act.Should().Throw<ScooterAlreadyTakenException>().WithMessage("Scooter with id 1 is already in rent");
        }

        [TestMethod]
        public void StartRent_ScooterDoesNotExist_ThrowsScooterDoesnotExistException()
        {
            //Arrange
            _scooterServiceMock.Setup(s => s.GetScooterById("0")).Throws<ScooterDoesnotExistException>();

            //Act
            Action act = () => _company.StartRent("0");

            //Assert
            act.Should().Throw<ScooterDoesnotExistException>().WithMessage("Scooter doesn't exist");
        }

        [TestMethod]
        public void StartRent_ScooterNullOrEmptyId_ThrowsInvalidIdException()
        {
            //Arrange
            _scooterServiceMock.Setup(s => s.GetScooterById("")).Throws<InvalidIdException>();

            //Act
            Action act = () => _company.StartRent("");

            //Assert
            act.Should().Throw<InvalidIdException>().WithMessage("Id cannot be null or empty");
        }

        [TestMethod]
        public void EndRent_ScooterWasRented_EndRentOfScooter()
        {
            //Arrange
            _scooterServiceMock.Setup(s => s.GetScooterById("1")).Returns(_defaultScooter);
            var rentedScooter = new RentedScooter("1", new DateTime(2020, 9, 8, 13, 30, 0), 1);
            _defaultScooter.IsRented = true;
            _activeRentScooterList.Add(rentedScooter);

            //Act
            var result = _company.EndRent("1");

            //Assert
            _defaultScooter.IsRented.Should().BeFalse();
            rentedScooter.EndTime.HasValue.Should().BeTrue();
            _activeRentScooterList.Count.Should().Be(0);
        }

        [TestMethod]
        public void EndRent_ScooterWasNotRented_ThrowScooterIsNotRentedException()
        {
            //Arrange
            _scooterServiceMock.Setup(s => s.GetScooterById("1")).Returns(_defaultScooter);
            var rentedScooter = new RentedScooter("1", DateTime.Now.AddMinutes(-10), 1);
            _defaultScooter.IsRented = false;

            //Act
            Action act = () => _company.EndRent("1");

            // Assert
            act.Should().Throw<ScooterIsNotRentedException>().WithMessage("Scooter with id 1 was not rented");
        }

        [TestMethod]
        public void CalculateIncome_CalculateIncome_YearNoValue_OnlyCompletedRentals_GetAllIncome()
        {
            //Arrange
            _accountingMock.Setup(accounting => accounting.GetReport(null)).Returns(234m);

            //Act
            var result = _company.CalculateIncome(null, false);

            //Assert
            result.Should().Be(234m);
        }

        [TestMethod]
        public void CalculateIncome_YearNoValue_IncludeNotCompletedRentals_GetAllIncome()
        {
            //Arrange
            _accountingMock.Setup(accounting => accounting.GetReport(null)).Returns(2m);
            _accountingMock.Setup(accounting => accounting.CalculateNotCompletedRentalIncome(_activeRentScooterList)).Returns(34);

            //Act
            var result = _company.CalculateIncome(null, true);
            
            //Assert
            result.Should().Be(36m);
        }

        [TestMethod]
        public void CalculateIncome_YearHasValue_OnlyCompletedRentals_GetIncomeForExpectedYear()
        {
            //Arrange
            _accountingMock.Setup(accounting => accounting.GetReport(2020)).Returns(2m);

            //Act
            var result = _company.CalculateIncome(2020, false);

            //Assert
            result.Should().Be(2m);
        }

        [TestMethod]
        public void CalculateIncome_YearHasValue_IncludeNotCompletedRentalsIsTrue_GetIncomeForExpectedYear()
        {
            //Arrange
            _accountingMock.Setup(accounting => accounting.GetReport(2020)).Returns(2m);
            _accountingMock.Setup(accounting => accounting.CalculateNotCompletedRentalIncome(_activeRentScooterList)).Returns(34);

            //Act
            var result = _company.CalculateIncome(2020, true);

            //Assert
            result.Should().Be(2m);
        }
    }
}
