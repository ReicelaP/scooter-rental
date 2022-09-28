using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ScooterRental.Tests
{
    [TestClass]
    public class DatabaseTests
    {
        private Database _database;
        private Dictionary<int, List<decimal>> _incomeData;

        [TestInitialize]
        public void Setup()
        {
            _incomeData = new Dictionary<int, List<decimal>>();
            _database = new Database(_incomeData);
        }

        [TestMethod]
        public void AddToDatabase_AddNewKeyValuePair_AddKeyAndValueToDictionary()
        {
            //Act
            _database.AddToDatabase(2000, 2.3m);
            _database.AddToDatabase(2001, 2.3m);

            //Assert
            _incomeData.Count.Should().Be(2);
        }

        [TestMethod]
        public void AddToDatabase_AddValueToExistingKey_AddValueToDictionary()
        {
            //Arrange
            List<decimal> rentTotalPriceList = new List<decimal>();
            rentTotalPriceList.Add(6.5m);
            _incomeData.Add(2000, rentTotalPriceList);

            //Act
            _database.AddToDatabase(2000, 2.3m);

            //Assert
            _incomeData.Count.Should().Be(1);
            rentTotalPriceList.Count.Should().Be(2);
        }
    }
}
