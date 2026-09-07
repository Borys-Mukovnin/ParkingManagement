using FluentAssertions;
using ParkingManagement.Domain.Aggregates.ParkingLocationAggregate;
using System;
using System.Linq;
using Xunit;

namespace ParkingManagement.Domain.Tests.Aggregates
{
    public class ParkingLocationTests
    {
        [Fact]
        public void Constructor_ShouldInitializeParkingLocationWithCorrectValues()
        {
            // Arrange & Act
            var parkingLocation = new ParkingLocation(
                name: "Arsenalna Metro Parking",
                country: "Ukraine",
                city: "Kyiv",
                zipcode: "01011",
                street: "Ivana Mazepy Street 11",
                amountSlots: 120,
                tarifRate: 50.0
            );

            // Assert
            parkingLocation.Id.Should().NotBe(Guid.Empty);
            parkingLocation.Name.Should().Be("Arsenalna Metro Parking");
            parkingLocation.Country.Should().Be("Ukraine");
            parkingLocation.City.Should().Be("Kyiv");
            parkingLocation.ZipCode.Should().Be("01011");
            parkingLocation.Street.Should().Be("Ivana Mazepy Street 11");
            parkingLocation.AmountSlots.Should().Be(120);
            parkingLocation.TarifRate.Should().Be(50.0);
            parkingLocation.Vehicles.Should().BeEmpty();
        }

        [Fact]
        public void Constructor_ShouldGenerateUniqueIds()
        {
            // Arrange & Act
            var location1 = new ParkingLocation(
                "Kontraktova Ploshcha",
                "Ukraine",
                "Kyiv",
                "04070",
                "Sahaidachnoho Street 6",
                80,
                45.0
            );

            var location2 = new ParkingLocation(
                "Troieshchyna Parking",
                "Ukraine",
                "Kyiv",
                "02000",
                "Heroiv Dnipra Street 1",
                100,
                40.0
            );

            // Assert
            location1.Id.Should().NotBe(location2.Id);
        }

        [Fact]
        public void LetVehicleIn_ShouldAddVehicleToCollection()
        {
            // Arrange
            var parkingLocation = new ParkingLocation(
                "Pecherska Lavra Parking",
                "Ukraine",
                "Kyiv",
                "01015",
                "Lavrska Street 15",
                50,
                55.0
            );

            var vehicle = new Vehicle("AA1111BB", "Mercedes-Benz E-Class");

            // Act
            parkingLocation.LetVehicleIn(vehicle);

            // Assert
            parkingLocation.Vehicles.Should().HaveCount(1);
            parkingLocation.Vehicles.Should().Contain(vehicle);
        }

        [Fact]
        public void LetVehicleIn_ShouldSetLastEnteredTime()
        {
            // Arrange
            var parkingLocation = new ParkingLocation(
                "Olimpiyska Stadium",
                "Ukraine",
                "Kyiv",
                "03150",
                "Velyka Vasylkivska Street 55",
                200,
                60.0
            );

            var vehicle = new Vehicle("KA2222IE", "Audi A6");
            var beforeEntry = DateTime.Now;

            // Act
            parkingLocation.LetVehicleIn(vehicle);

            // Assert
            vehicle.LastEntered.Should().BeOnOrAfter(beforeEntry);
            vehicle.LastEntered.Should().BeOnOrBefore(DateTime.Now);
        }

        [Fact]
        public void LetVehicleIn_ShouldThrowException_WhenParkingIsFull()
        {
            // Arrange
            var parkingLocation = new ParkingLocation(
                "Lukianivska Parking",
                "Ukraine",
                "Kyiv",
                "04112",
                "Dehtyarivska Street 25",
                2,
                48.0
            );

            var vehicle1 = new Vehicle("AA3333BB", "Toyota Corolla");
            var vehicle2 = new Vehicle("KA4444IE", "Honda Civic");
            var vehicle3 = new Vehicle("AA5555BB", "Nissan Qashqai");

            parkingLocation.LetVehicleIn(vehicle1);
            parkingLocation.LetVehicleIn(vehicle2);

            // Act
            Action act = () => parkingLocation.LetVehicleIn(vehicle3);

            // Assert
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("No available parking slots.");
            parkingLocation.Vehicles.Should().HaveCount(2);
        }

        [Fact]
        public void LetVehicleIn_ShouldAllowMultipleVehicles_WhenSlotsAvailable()
        {
            // Arrange
            var parkingLocation = new ParkingLocation(
                "Darnytsia Mall",
                "Ukraine",
                "Kyiv",
                "02093",
                "Kharkivske Highway 201",
                150,
                42.0
            );

            var vehicle1 = new Vehicle("AA6666BB", "Ford Focus");
            var vehicle2 = new Vehicle("KA7777IE", "Hyundai Tucson");
            var vehicle3 = new Vehicle("AA8888BB", "Mazda CX-5");

            // Act
            parkingLocation.LetVehicleIn(vehicle1);
            parkingLocation.LetVehicleIn(vehicle2);
            parkingLocation.LetVehicleIn(vehicle3);

            // Assert
            parkingLocation.Vehicles.Should().HaveCount(3);
            parkingLocation.Vehicles.Should().Contain([vehicle1, vehicle2, vehicle3]);
        }

        [Fact]
        public void LetVehicleOut_ShouldRemoveVehicleFromCollection()
        {
            // Arrange
            var parkingLocation = new ParkingLocation(
                "Respublikansky Stadium",
                "Ukraine",
                "Kyiv",
                "03057",
                "Velyka Vasylkivska Street 55",
                100,
                50.0
            );

            var vehicle = new Vehicle("KA9999IE", "Volkswagen Golf");
            parkingLocation.LetVehicleIn(vehicle);

            // Act
            parkingLocation.LetVehicleOut(vehicle);

            // Assert
            parkingLocation.Vehicles.Should().BeEmpty();
        }

        [Fact]
        public void LetVehicleOut_ShouldReturnCorrectParkingFee()
        {
            // Arrange
            var parkingLocation = new ParkingLocation(
                "Khreshchatyk Central",
                "Ukraine",
                "Kyiv",
                "01001",
                "Khreshchatyk Street 10",
                80,
                50.0
            );

            var vehicle = new Vehicle("AA0000BB", "Renault Megane");
            parkingLocation.LetVehicleIn(vehicle);

            // Simulate 3 hours of parking
            vehicle.LastEntered = DateTime.Now.AddHours(-3);

            // Act
            var fee = parkingLocation.LetVehicleOut(vehicle);

            // Assert
            fee.Should().Be(150.0); // 3 hours * 50 UAH
        }

        [Fact]
        public void LetVehicleOut_ShouldThrowException_WhenVehicleNotFound()
        {
            // Arrange
            var parkingLocation = new ParkingLocation(
                "Pozniaky Residential",
                "Ukraine",
                "Kyiv",
                "02660",
                "Hryhoriya Skovorody Avenue 3",
                60,
                38.0
            );

            var vehicle = new Vehicle("KA1212AA", "Skoda Superb");

            // Act
            Action act = () => parkingLocation.LetVehicleOut(vehicle);

            // Assert
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("Vehicle not found in this parking location.");
        }

        [Fact]
        public void LetVehicleOut_ShouldOnlyRemoveSpecifiedVehicle()
        {
            // Arrange
            var parkingLocation = new ParkingLocation(
                "Shuliavska Metro",
                "Ukraine",
                "Kyiv",
                "04116",
                "Shuliavska Street 1",
                90,
                46.0
            );

            var vehicle1 = new Vehicle("AA3434BB", "Peugeot 308");
            var vehicle2 = new Vehicle("KA5656IE", "Citroen C4");
            var vehicle3 = new Vehicle("AA7878BB", "Opel Astra");

            parkingLocation.LetVehicleIn(vehicle1);
            parkingLocation.LetVehicleIn(vehicle2);
            parkingLocation.LetVehicleIn(vehicle3);

            // Act
            parkingLocation.LetVehicleOut(vehicle2);

            // Assert
            parkingLocation.Vehicles.Should().HaveCount(2);
            parkingLocation.Vehicles.Should().Contain(vehicle1);
            parkingLocation.Vehicles.Should().NotContain(vehicle2);
            parkingLocation.Vehicles.Should().Contain(vehicle3);
        }

        [Fact]
        public void LetVehicleOut_ShouldCalculateFeeBasedOnParkingLocationTarif()
        {
            // Arrange
            var expensiveParking = new ParkingLocation(
                "Bessarabska Square Premium",
                "Ukraine",
                "Kyiv",
                "01004",
                "Bessarabska Square 2",
                40,
                80.0 // Expensive rate
            );

            var cheapParking = new ParkingLocation(
                "Livoberezhna Budget",
                "Ukraine",
                "Kyiv",
                "02174",
                "Mykoly Zakrevskogo Street 97",
                150,
                30.0 // Cheap rate
            );

            var vehicle1 = new Vehicle("AA1010BB", "BMW 3 Series");
            var vehicle2 = new Vehicle("KA2020IE", "Lada Vesta");

            expensiveParking.LetVehicleIn(vehicle1);
            cheapParking.LetVehicleIn(vehicle2);

            // Simulate 2 hours of parking for both
            vehicle1.LastEntered = DateTime.Now.AddHours(-2);
            vehicle2.LastEntered = DateTime.Now.AddHours(-2);

            // Act
            var expensiveFee = expensiveParking.LetVehicleOut(vehicle1);
            var cheapFee = cheapParking.LetVehicleOut(vehicle2);

            // Assert
            expensiveFee.Should().Be(160.0); // 2 hours * 80 UAH
            cheapFee.Should().Be(60.0);       // 2 hours * 30 UAH
        }
    }
}