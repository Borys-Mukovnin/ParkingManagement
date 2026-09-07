using FluentAssertions;
using Moq;
using ParkingManagement.Application.Dtos.ParkingLocation;
using ParkingManagement.Application.Services;
using ParkingManagement.Domain.Aggregates.ParkingLocationAggregate;
using ParkingManagement.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ParkingManagement.Application.Tests.Services
{
    public class ParkingLocationServiceTests
    {
        private readonly Mock<IParkingLocationRepository> _repositoryMock;
        private readonly ParkingLocationService _service;

        public ParkingLocationServiceTests()
        {
            _repositoryMock = new Mock<IParkingLocationRepository>();
            _service = new ParkingLocationService(_repositoryMock.Object);
        }

        [Fact]
        public void GetParkingLocations_ShouldReturnAllLocations_WhenNoFilterProvided()
        {
            // Arrange
            var location1 = new ParkingLocation(
                "Sofiyivska Square",
                "Ukraine",
                "Kyiv",
                "01001",
                "Sofiyivska Square 1",
                100,
                50.0
            );

            var location2 = new ParkingLocation(
                "Poshtova Square",
                "Ukraine",
                "Kyiv",
                "04071",
                "Poshtova Square 2",
                80,
                45.0
            );

            var locations = new List<ParkingLocation> { location1, location2 };

            _repositoryMock
                .Setup(r => r.Get(null))
                .Returns(locations);

            // Act
            var result = _service.GetParkingLocations();

            // Assert
            result.Should().HaveCount(2);
            result.Should().Contain(dto => dto.name == "Sofiyivska Square");
            result.Should().Contain(dto => dto.name == "Poshtova Square");
            _repositoryMock.Verify(r => r.Get(null), Times.Once);
        }

        [Fact]
        public void GetParkingLocations_ShouldReturnFilteredLocations_WhenFilterProvided()
        {
            // Arrange
            var location1 = new ParkingLocation(
                "Maidan Nezalezhnosti",
                "Ukraine",
                "Kyiv",
                "01001",
                "Maidan Nezalezhnosti 1",
                150,
                60.0
            );

            var location2 = new ParkingLocation(
                "Троєщина",
                "Ukraine",
                "Kyiv",
                "02000",
                "Heroiv Dnipra 32",
                50,
                35.0
            );

            Expression<Func<ParkingLocation, bool>> filter = pl => pl.AmountSlots > 100;
            var filteredLocations = new List<ParkingLocation> { location1 };

            _repositoryMock
                .Setup(r => r.Get(It.IsAny<Expression<Func<ParkingLocation, bool>>>()))
                .Returns(filteredLocations);

            // Act
            var result = _service.GetParkingLocations(filter);

            // Assert
            result.Should().HaveCount(1);
            result.First().name.Should().Be("Maidan Nezalezhnosti");
            result.First().amountSlots.Should().Be(150);
            _repositoryMock.Verify(r => r.Get(It.IsAny<Expression<Func<ParkingLocation, bool>>>()), Times.Once);
        }

        [Fact]
        public void GetParkingLocations_ShouldReturnEmptyCollection_WhenNoLocationsExist()
        {
            // Arrange
            _repositoryMock
                .Setup(r => r.Get(null))
                .Returns(new List<ParkingLocation>());

            // Act
            var result = _service.GetParkingLocations();

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void GetParkingLocations_ShouldMapVehiclesCorrectly()
        {
            // Arrange
            var parkingLocation = new ParkingLocation(
                "Pechersk",
                "Ukraine",
                "Kyiv",
                "01030",
                "Druzhby Narodiv Boulevard 5",
                120,
                55.0
            );

            var vehicle1 = new Vehicle("AA1234BB", "Tesla Model S");
            var vehicle2 = new Vehicle("KA5678IE", "BMW i4");

            // Simulate vehicles being in the parking location
            typeof(ParkingLocation)
                .GetProperty("Vehicles")
                .SetValue(parkingLocation, new List<Vehicle> { vehicle1, vehicle2 });

            _repositoryMock
                .Setup(r => r.Get(null))
                .Returns(new List<ParkingLocation> { parkingLocation });

            // Act
            var result = _service.GetParkingLocations();

            // Assert
            var dto = result.First();
            dto.vehicles.Should().HaveCount(2);
            dto.vehicles.Should().Contain(v => v.licensePlate == "AA1234BB");
            dto.vehicles.Should().Contain(v => v.licensePlate == "KA5678IE");
        }

        [Fact]
        public void GetParkingLocationById_ShouldReturnCorrectLocation()
        {
            // Arrange
            var id = Guid.NewGuid();
            var parkingLocation = new ParkingLocation(
                "Khreshchatyk",
                "Ukraine",
                "Kyiv",
                "01001",
                "Khreshchatyk Street 15",
                100,
                50.0
            );

            // Set the Id using reflection since it has a setter
            typeof(ParkingLocation).GetProperty("Id").SetValue(parkingLocation, id);

            _repositoryMock
                .Setup(r => r.GetById(id))
                .Returns(parkingLocation);

            // Act
            var result = _service.GetParkingLocationById(id);

            // Assert
            result.id.Should().Be(id);
            result.name.Should().Be("Khreshchatyk");
            result.city.Should().Be("Kyiv");
            result.amountSlots.Should().Be(100);
            result.TarifRate.Should().Be(50.0);
            _repositoryMock.Verify(r => r.GetById(id), Times.Once);
        }

        [Fact]
        public void GetParkingLocationById_ShouldMapAllProperties()
        {
            // Arrange
            var id = Guid.NewGuid();
            var parkingLocation = new ParkingLocation(
                "Obolon",
                "Ukraine",
                "Kyiv",
                "04205",
                "Obolonsky Avenue 26",
                180,
                48.0
            );

            typeof(ParkingLocation).GetProperty("Id").SetValue(parkingLocation, id);

            _repositoryMock
                .Setup(r => r.GetById(id))
                .Returns(parkingLocation);

            // Act
            var result = _service.GetParkingLocationById(id);

            // Assert
            result.id.Should().Be(id);
            result.name.Should().Be("Obolon");
            result.country.Should().Be("Ukraine");
            result.city.Should().Be("Kyiv");
            result.zipcode.Should().Be("04205");
            result.street.Should().Be("Obolonsky Avenue 26");
            result.amountSlots.Should().Be(180);
            result.TarifRate.Should().Be(48.0);
        }

        [Fact]
        public void AddParkingLocation_ShouldCallRepositoryAdd()
        {
            // Arrange
            var dto = new ParkingLocationAddDto(
                name: "Shuliavska",
                country: "Ukraine",
                city: "Kyiv",
                zipcode: "04116",
                street: "Shuliavska Street 2",
                amountSlots: 90,
                tarifRate: 42.0
            );

            _repositoryMock
                .Setup(r => r.Add(It.IsAny<ParkingLocation>()))
                .Verifiable();

            // Act
            _service.AddParkingLocation(dto);

            // Assert
            _repositoryMock.Verify(r => r.Add(It.Is<ParkingLocation>(pl =>
                pl.Name == "Shuliavska" &&
                pl.Country == "Ukraine" &&
                pl.City == "Kyiv" &&
                pl.ZipCode == "04116" &&
                pl.Street == "Shuliavska Street 2" &&
                pl.AmountSlots == 90 &&
                pl.TarifRate == 42.0
            )), Times.Once);
        }

        [Fact]
        public void AddParkingLocation_ShouldCreateEntityWithGeneratedId()
        {
            // Arrange
            var dto = new ParkingLocationAddDto(
                "Shuliavska",
                "Ukraine",
                "Kyiv",
                "04112",
                "Dehtyarivska Street 30",
                70,
                40.0
            );

            ParkingLocation capturedLocation = null;
            _repositoryMock
                .Setup(r => r.Add(It.IsAny<ParkingLocation>()))
                .Callback<ParkingLocation>(pl => capturedLocation = pl);

            // Act
            _service.AddParkingLocation(dto);

            // Assert
            capturedLocation.Should().NotBeNull();
            capturedLocation.Id.Should().NotBe(Guid.Empty);
        }

        [Fact]
        public void RemoveParkingLocation_ShouldCallRepositoryRemove_WhenLocationExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            var parkingLocation = new ParkingLocation(
                "Darnytsa",
                "Ukraine",
                "Kyiv",
                "02093",
                "Kharkivske Highway 201",
                150,
                45.0
            );

            typeof(ParkingLocation).GetProperty("Id").SetValue(parkingLocation, id);

            _repositoryMock
                .Setup(r => r.GetById(id))
                .Returns(parkingLocation);

            _repositoryMock
                .Setup(r => r.Remove(parkingLocation))
                .Verifiable();

            // Act
            _service.RemoveParkingLocation(id);

            // Assert
            _repositoryMock.Verify(r => r.GetById(id), Times.Once);
            _repositoryMock.Verify(r => r.Remove(parkingLocation), Times.Once);
        }

        [Fact]
        public void RemoveParkingLocation_ShouldNotCallRepositoryRemove_WhenLocationDoesNotExist()
        {
            // Arrange
            var id = Guid.NewGuid();

            _repositoryMock
                .Setup(r => r.GetById(id))
                .Returns((ParkingLocation)null);

            // Act
            _service.RemoveParkingLocation(id);

            // Assert
            _repositoryMock.Verify(r => r.GetById(id), Times.Once);
            _repositoryMock.Verify(r => r.Remove(It.IsAny<ParkingLocation>()), Times.Never);
        }

        [Fact]
        public void RemoveParkingLocation_ShouldHandleNullGracefully()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            _repositoryMock
                .Setup(r => r.GetById(nonExistentId))
                .Returns((ParkingLocation)null);

            // Act
            Action act = () => _service.RemoveParkingLocation(nonExistentId);

            // Assert
            act.Should().NotThrow();
        }
    }
}