using FluentAssertions;
using ParkingManagement.Domain.Aggregates.ParkingLocationAggregate;
using System;
using Xunit;

namespace ParkingManagement.Domain.Tests.Aggregates
{
    public class VehicleTests
    {
        [Fact]
        public void Constructor_ShouldInitializeVehicleWithCorrectValues()
        {
            // Arrange & Act
            var vehicle = new Vehicle("AA1234BB", "Tesla Model 3");

            // Assert
            vehicle.Id.Should().NotBe(Guid.Empty);
            vehicle.LicensePlate.Should().Be("AA1234BB");
            vehicle.Model.Should().Be("Tesla Model 3");
        }

        [Fact]
        public void Constructor_ShouldGenerateUniqueIds()
        {
            // Arrange & Act
            var vehicle1 = new Vehicle("KA5678IE", "Porsche Cayenne");
            var vehicle2 = new Vehicle("AA9012BB", "Range Rover");

            // Assert
            vehicle1.Id.Should().NotBe(vehicle2.Id);
        }

        [Fact]
        public void CalculateParkingFee_ShouldReturnCorrectFee_ForExactHours()
        {
            // Arrange
            var vehicle = new Vehicle("AA1111BB", "Volvo XC90");
            vehicle.LastEntered = new DateTime(2026, 1, 17, 10, 0, 0);
            var exitTime = new DateTime(2026, 1, 17, 14, 0, 0); // 4 hours later
            var tarifRate = 50.0;

            // Act
            var fee = vehicle.CalculateParkingFee(exitTime, tarifRate);

            // Assert
            fee.Should().Be(200.0); // 4 hours * 50 UAH
        }

        [Fact]
        public void CalculateParkingFee_ShouldRoundUpPartialHours()
        {
            // Arrange
            var vehicle = new Vehicle("KA2222IE", "Lexus RX");
            vehicle.LastEntered = new DateTime(2026, 1, 17, 10, 0, 0);
            var exitTime = new DateTime(2026, 1, 17, 12, 30, 0); // 2.5 hours later
            var tarifRate = 40.0;

            // Act
            var fee = vehicle.CalculateParkingFee(exitTime, tarifRate);

            // Assert
            fee.Should().Be(120.0); // 3 hours (rounded up) * 40 UAH
        }

        [Fact]
        public void CalculateParkingFee_ShouldRoundUpEvenOneMinute()
        {
            // Arrange
            var vehicle = new Vehicle("AA3333BB", "Jaguar F-Pace");
            vehicle.LastEntered = new DateTime(2026, 1, 17, 9, 0, 0);
            var exitTime = new DateTime(2026, 1, 17, 10, 1, 0); // 1 hour 1 minute
            var tarifRate = 60.0;

            // Act
            var fee = vehicle.CalculateParkingFee(exitTime, tarifRate);

            // Assert
            fee.Should().Be(120.0); // 2 hours (rounded up) * 60 UAH
        }

        [Fact]
        public void CalculateParkingFee_ShouldHandleDifferentTarifRates()
        {
            // Arrange
            var vehicle = new Vehicle("KA4444IE", "Mitsubishi Outlander");
            vehicle.LastEntered = new DateTime(2026, 1, 17, 8, 0, 0);
            var exitTime = new DateTime(2026, 1, 17, 11, 0, 0); // 3 hours

            // Act
            var cheapFee = vehicle.CalculateParkingFee(exitTime, 25.0);
            var expensiveFee = vehicle.CalculateParkingFee(exitTime, 100.0);

            // Assert
            cheapFee.Should().Be(75.0);   // 3 hours * 25 UAH
            expensiveFee.Should().Be(300.0); // 3 hours * 100 UAH
        }

        [Fact]
        public void CalculateParkingFee_ShouldCalculateForLongStay()
        {
            // Arrange
            var vehicle = new Vehicle("AA5555BB", "Subaru Forester");
            vehicle.LastEntered = new DateTime(2026, 1, 16, 10, 0, 0);
            var exitTime = new DateTime(2026, 1, 17, 10, 0, 0); // 24 hours later
            var tarifRate = 45.0;

            // Act
            var fee = vehicle.CalculateParkingFee(exitTime, tarifRate);

            // Assert
            fee.Should().Be(1080.0); // 24 hours * 45 UAH
        }

        [Fact]
        public void CalculateParkingFee_ShouldHandleShortStay()
        {
            // Arrange
            var vehicle = new Vehicle("KA6666IE", "Kia Sportage");
            vehicle.LastEntered = new DateTime(2026, 1, 17, 14, 0, 0);
            var exitTime = new DateTime(2026, 1, 17, 14, 15, 0); // 15 minutes
            var tarifRate = 50.0;

            // Act
            var fee = vehicle.CalculateParkingFee(exitTime, tarifRate);

            // Assert
            fee.Should().Be(50.0); // 1 hour (rounded up) * 50 UAH
        }

        [Fact]
        public void CalculateParkingFee_ShouldReturnZero_ForZeroTarif()
        {
            // Arrange - Free parking scenario
            var vehicle = new Vehicle("AA7777BB", "Chevrolet Bolt");
            vehicle.LastEntered = new DateTime(2026, 1, 17, 12, 0, 0);
            var exitTime = new DateTime(2026, 1, 17, 15, 0, 0); // 3 hours
            var tarifRate = 0.0;

            // Act
            var fee = vehicle.CalculateParkingFee(exitTime, tarifRate);

            // Assert
            fee.Should().Be(0.0);
        }

        [Fact]
        public void CalculateParkingFee_ShouldHandleMultipleDays()
        {
            // Arrange
            var vehicle = new Vehicle("KA8888IE", "Nissan Leaf");
            vehicle.LastEntered = new DateTime(2026, 1, 15, 9, 0, 0);
            var exitTime = new DateTime(2026, 1, 17, 17, 30, 0); // 2 days 8.5 hours
            var tarifRate = 35.0;

            // Act
            var fee = vehicle.CalculateParkingFee(exitTime, tarifRate);

            // Assert
            var expectedHours = 57; // Math.Ceiling(56.5) = 57
            fee.Should().Be(expectedHours * 35.0); // 1995.0 UAH
        }

        [Fact]
        public void CalculateParkingFee_ShouldRoundUp_ForExactlyHalfHour()
        {
            // Arrange
            var vehicle = new Vehicle("AA9999BB", "Alfa Romeo Giulia");
            vehicle.LastEntered = new DateTime(2026, 1, 17, 10, 0, 0);
            var exitTime = new DateTime(2026, 1, 17, 10, 30, 0); // Exactly 0.5 hours
            var tarifRate = 55.0;

            // Act
            var fee = vehicle.CalculateParkingFee(exitTime, tarifRate);

            // Assert
            fee.Should().Be(55.0); // 1 hour (rounded up) * 55 UAH
        }
    }
}