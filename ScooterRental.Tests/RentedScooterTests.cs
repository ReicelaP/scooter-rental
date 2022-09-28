using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ScooterRental.Tests
{
    [TestClass]
    public class RentedScooterTests
    {
        private RentedScooter _rentedScooter;

        [TestMethod]
        public void RentedScooterCreation_IdAndStartTimeAndPricePerMinuteSetCorrectly()
        {
            _rentedScooter = new RentedScooter("2", new DateTime(2021, 2, 14, 10, 12, 23), 2.4m);
            _rentedScooter.Id.Should().Be("2");
            _rentedScooter.StartTime.Should().Be(new DateTime(2021, 2, 14, 10, 12, 23));
            _rentedScooter.PricePerMinute.Should().Be(2.4m);
        }
    }
}
