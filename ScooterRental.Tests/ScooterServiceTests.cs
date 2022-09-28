using System;
using System.Linq;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScooterRental.Interfaces;
using ScooterRental.Exceptions;

namespace ScooterRental.Tests
{
    [TestClass]
    public class ScooterServiceTests
    {
        private IScooterService _scooterService;
        private List<Scooter> _inventory;

        [TestInitialize]
        public void SetUp()
        {
            _inventory = new List<Scooter>();
            _scooterService = new ScooterService(_inventory);
        }

        [TestMethod]
        public void AddScooter_AddValidScooter_ScooterAdded()
        {
            //Act
            _scooterService.AddScooter("1", 0.2m);

            //Assert
            _inventory.Count.Should().Be(1);
        }

        [TestMethod]
        public void AddScooter_AddScooterTwice_ThrowsDuplicateScooterException()
        {
            //Arrange
            _scooterService.AddScooter("1", 0.2m);

            //Act
            Action act = () => _scooterService.AddScooter("1", 0.2m);

            //Assert
            act.Should().Throw<DuplicateScooterException>().WithMessage("Scooter with id 1 already exists");
        }

        [TestMethod]
        public void AddScooter_AddScooterWithPriceZeroOrLess_ThrowsInvalidPriceException()
        {
            //Act
            Action act = () => _scooterService.AddScooter("1", -0.2m);

            //Assert
            act.Should().Throw<InvalidPriceException>().WithMessage("Given price -0.2 not valid");
        }

        [TestMethod]
        public void AddScooter_AddScooterNullOrEmptyId_ThrowsInvalidIdException()
        {
            //Act
            Action act = () => _scooterService.AddScooter("", 0.2m);

            //Assert
            act.Should().Throw<InvalidIdException>().WithMessage("Id cannot be null or empty");
        }

        [TestMethod]
        public void RemoveScooter_ScooterExists_ScooterIsRemoved()
        {
            //Arrange
            _inventory.Add(new Scooter("1", 0.2m));

            //Act
            _scooterService.RemoveScooter("1");

            //Assert
            _inventory.Any(scooter => scooter.Id == "1").Should().BeFalse();
            _inventory.Count.Should().Be(0);
        }

        [TestMethod]
        public void RemoveScooter_ScooterDoesnotExist_ThrowsScooterDoesnotExistException()
        {
            //Act
            Action act = () => _scooterService.RemoveScooter("1");

            //Assert
            act.Should().Throw<ScooterDoesnotExistException>().WithMessage("Scooter with id 1 doesn't exist");
        }

        [TestMethod]
        public void RemoveScooter_RemoveScooterNullOrEmptyId_ThrowsInvalidIdException()
        {
            //Act
            Action act = () => _scooterService.RemoveScooter("");

            //Assert
            act.Should().Throw<InvalidIdException>().WithMessage("Id cannot be null or empty");
        }

        [TestMethod]
        public void GetScooters_ScootersExist_ReturnListOfScooters()
        {
            //Arrange
            _inventory.Add(new Scooter("1", 0.2m));
            _inventory.Add(new Scooter("2", 0.2m));
            var list = new List<Scooter>() { new Scooter("1", 0.2m), new Scooter("2", 0.2m) };

            //Act
            _scooterService.GetScooters();

            //Assert
            _inventory.Should().BeEquivalentTo(list);
        }

        [TestMethod]
        public void GetScoterById_ScooterExists_GetScooter()
        {
            //Arrange
            _inventory.Add(new Scooter("1", 0.2m));

            //Act
            var scooter = _scooterService.GetScooterById("1");

            //Assert
            scooter.Id.Should().Be("1");
            scooter.PricePerMinute.Should().Be(0.2m);
        }

        [TestMethod]
        public void GetScooterById_ScooterDoesNotExist_ThrowsScooterDoesnotExistException()
        {
            //Act
            Action act = () => _scooterService.GetScooterById("1");

            //Assert
            act.Should().Throw<ScooterDoesnotExistException>().WithMessage("Scooter with id 1 doesn't exist");
        }

        [TestMethod]
        public void GetScooterById_GetScooterNullOrEmptyId_ThrowsInvalidIdException()
        {
            //Act
            Action act = () => _scooterService.GetScooterById("");

            //Assert
            act.Should().Throw<InvalidIdException>().WithMessage("Id cannot be null or empty");
        }
    }
}
