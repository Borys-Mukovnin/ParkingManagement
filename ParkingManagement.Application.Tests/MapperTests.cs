using FluentAssertions;
using ParkingManagement.Application.Dtos.ParkingLocation;
using ParkingManagement.Application.Dtos.Vehicle;
using ParkingManagement.Domain.Aggregates.ParkingLocationAggregate;
using System;
using System.Linq;
using Xunit;

namespace ParkingManagement.Application.Tests
{
    public class MapperTests
    {
        [Fact]
        public void ParkingLocationAddDto_ToEntity_ShouldMapToParkingLocation()
        {
            // Arrange
            var dto = new ParkingLocationAddDto(
                name: "Kontraktova Square",
                country: "Ukraine",
                city: "Kyiv",
                zipcode: "04070",
                street: "Kontraktova Square 1",
                amountSlots: 85,
                tarifRate: 47.0
            );

            // Act
            var parkingLocation = dto.ToEntity();

            // Assert
            parkingLocation.Should().NotBeNull();
            parkingLocation.Id.Should().NotBe(Guid.Empty);
            parkingLocation.Name.Should().Be("Kontraktova Square");
            parkingLocation.Country.Should().Be("Ukraine");
            parkingLocation.City.Should().Be("Kyiv");
            parkingLocation.ZipCode.Should().Be("04070");
            parkingLocation.Street.Should().Be("Kontraktova Square 1");
            parkingLocation.AmountSlots.Should().Be(85);
            parkingLocation.TarifRate.Should().Be(47.0);
            parkingLocation.Vehicles.Should().BeEmpty();
        }

        [Fact]
        public void ParkingLocation_ToDto_ShouldMapToParkingLocationDto()
        {
            // Arrange
            var parkingLocation = new ParkingLocation(
                "Golden Gates",
                "Ukraine",
                "Kyiv",
                "01034",
                "Volodymyrska Street 40A",
                60,
                52.0
            );

            // Act
            var dto = parkingLocation.ToDto();

            // Assert
            dto.Should().NotBeNull();
            dto.id.Should().Be(parkingLocation.Id);
            dto.name.Should().Be("Golden Gates");
            dto.country.Should().Be("Ukraine");
            dto.city.Should().Be("Kyiv");
            dto.zipcode.Should().Be("01034");
            dto.street.Should().Be("Volodymyrska Street 40A");
            dto.amountSlots.Should().Be(60);
            dto.TarifRate.Should().Be(52.0);
            dto.vehicles.Should().BeEmpty();
        }

        [Fact]
        public void ParkingLocation_ToDto_ShouldMapVehicles()
        {
            // Arrange
            var parkingLocation = new ParkingLocation(
                "Lybidska",
                "Ukraine",
                "Kyiv",
                "03039",
                "Peremohy Avenue 68",
                140,
                44.0
            );

            var vehicle1 = new Vehicle("AA1111BB", "Audi Q7");
            var vehicle2 = new Vehicle("KA2222IE", "Mercedes GLE");

            // Use reflection to add vehicles to the collection
            typeof(ParkingLocation)
                .GetProperty("Vehicles")
                .SetValue(parkingLocation, new[] { vehicle1, vehicle2 }.ToList());

            // Act
            var dto = parkingLocation.ToDto();

            // Assert
            dto.vehicles.Should().HaveCount(2);
            dto.vehicles.Should().Contain(v => v.licensePlate == "AA1111BB" && v.model == "Audi Q7");
            dto.vehicles.Should().Contain(v => v.licensePlate == "KA2222IE" && v.model == "Mercedes GLE");
        }

        [Fact]
        public void Vehicle_ToDto_ShouldMapToVehicleDto()
        {
            // Arrange
            var vehicle = new Vehicle("AA3333BB", "Porsche Taycan");
            var lastEntered = new DateTime(2026, 1, 17, 10, 30, 0);

            typeof(Vehicle).GetProperty("LastEntered").SetValue(vehicle, lastEntered);

            // Act
            var dto = vehicle.ToDto();

            // Assert
            dto.Should().NotBeNull();
            dto.id.Should().Be(vehicle.Id);
            dto.licensePlate.Should().Be("AA3333BB");
            dto.model.Should().Be("Porsche Taycan");
            dto.LastEntered.Should().Be(lastEntered);
        }

        [Fact]
        public void VehicleAddDto_ToEntity_ShouldMapToVehicle()
        {
            // Arrange
            var dto = new VehicleAddDto(
                licensePlate: "KA4444IE",
                model: "Volvo S90"
            );

            // Act
            var vehicle = dto.ToEntity();

            // Assert
            vehicle.Should().NotBeNull();
            vehicle.Id.Should().NotBe(Guid.Empty);
            vehicle.LicensePlate.Should().Be("KA4444IE");
            vehicle.Model.Should().Be("Volvo S90");
        }

        [Fact]
        public void VehicleAddDto_ToEntity_ShouldGenerateUniqueIds()
        {
            // Arrange
            var dto1 = new VehicleAddDto("AA5555BB", "Lexus ES");
            var dto2 = new VehicleAddDto("KA6666IE", "Infiniti Q50");

            // Act
            var vehicle1 = dto1.ToEntity();
            var vehicle2 = dto2.ToEntity();

            // Assert
            vehicle1.Id.Should().NotBe(vehicle2.Id);
        }

        [Fact]
        public void ParkingLocationAddDto_ToEntity_ShouldGenerateUniqueIds()
        {
            // Arrange
            var dto1 = new ParkingLocationAddDto(
                "Posnyaki",
                "Ukraine",
                "Kyiv",
                "02660",
                "Hryhoriya Skovorody Avenue 5",
                95,
                39.0
            );

            var dto2 = new ParkingLocationAddDto(
                "Academmisto",
                "Ukraine",
                "Kyiv",
                "03115",
                "Akademika Hlushkova Avenue 2",
                110,
                46.0
            );

            // Act
            var location1 = dto1.ToEntity();
            var location2 = dto2.ToEntity();

            // Assert
            location1.Id.Should().NotBe(location2.Id);
        }

        [Fact]
        public void CompleteMapping_ShouldPreserveAllData()
        {
            // Arrange
            var addDto = new ParkingLocationAddDto(
                "Teremky",
                "Ukraine",
                "Kyiv",
                "03022",
                "Akademika Zabolotnoho Street 20",
                130,
                43.0
            );

            // Act - Add -> Domain -> Dto
            var domainEntity = addDto.ToEntity();
            var resultDto = domainEntity.ToDto();

            // Assert
            resultDto.name.Should().Be(addDto.name);
            resultDto.country.Should().Be(addDto.country);
            resultDto.city.Should().Be(addDto.city);
            resultDto.zipcode.Should().Be(addDto.zipcode);
            resultDto.street.Should().Be(addDto.street);
            resultDto.amountSlots.Should().Be(addDto.amountSlots);
            resultDto.TarifRate.Should().Be(addDto.tarifRate);
        }
    }
}