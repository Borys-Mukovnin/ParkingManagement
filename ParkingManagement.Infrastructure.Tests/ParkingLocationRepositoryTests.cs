using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ParkingManagement.Domain.Aggregates.ParkingLocationAggregate;

namespace ParkingManagement.Infrastructure.Tests
{
    public class ParkingLocationRepositoryTests : IDisposable
    {
        private readonly ParkingLocationContext _context;
        private readonly ParkingLocationRepository _repository;

        public ParkingLocationRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ParkingLocationContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ParkingLocationContext(options);
            _repository = new ParkingLocationRepository(_context);
        }

        [Fact]
        public void Add_ShouldAddParkingLocationToDatabase()
        {
            // Arrange
            var parkingLocation = new ParkingLocation(
                name: "Khreshchatyk Parking",
                country: "Ukraine",
                city: "Kyiv",
                zipcode: "01001",
                street: "Khreshchatyk Street 1",
                amountSlots: 150,
                tarifRate: 50.0
            );

            // Act
            _repository.Add(parkingLocation);

            // Assert
            var result = _context.ParkingLocations.Find(parkingLocation.Id);
            result.Should().NotBeNull();
            result.Name.Should().Be("Khreshchatyk Parking");
            result.City.Should().Be("Kyiv");
            result.Street.Should().Be("Khreshchatyk Street 1");
            result.AmountSlots.Should().Be(150);
            result.TarifRate.Should().Be(50.0);
        }

        [Fact]
        public void GetById_ShouldReturnParkingLocationWithVehicles()
        {
            // Arrange
            var parkingLocation = new ParkingLocation(
                name: "Maidan Nezalezhnosti Parking",
                country: "Ukraine",
                city: "Kyiv",
                zipcode: "01001",
                street: "Maidan Nezalezhnosti 1",
                amountSlots: 100,
                tarifRate: 45.0
            );

            var vehicle1 = new Vehicle("AA1234BB", "Toyota Camry");
            var vehicle2 = new Vehicle("KA5678IE", "BMW X5");

            _context.ParkingLocations.Add(parkingLocation);
            _context.SaveChanges();

            vehicle1.ParkingLocationId = parkingLocation.Id;
            vehicle2.ParkingLocationId = parkingLocation.Id;
            _context.Vehicles.AddRange(vehicle1, vehicle2);
            _context.SaveChanges();

            // Act
            var result = _repository.GetById(parkingLocation.Id);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be("Maidan Nezalezhnosti Parking");
            result.Vehicles.Should().HaveCount(2);
            result.Vehicles.Should().Contain(v => v.LicensePlate == "AA1234BB");
            result.Vehicles.Should().Contain(v => v.LicensePlate == "KA5678IE");
        }

        [Fact]
        public void GetById_ShouldReturnNull_WhenParkingLocationDoesNotExist()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = _repository.GetById(nonExistentId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void Get_WithoutFilter_ShouldReturnAllParkingLocations()
        {
            // Arrange
            var location1 = new ParkingLocation(
                "Lviv City Center",
                "Ukraine",
                "Lviv",
                "79000",
                "Prospect Svobody 1",
                80,
                40.0
            );

            var location2 = new ParkingLocation(
                "Odesa Port Parking",
                "Ukraine",
                "Odesa",
                "65000",
                "Primorsky Boulevard 5",
                120,
                55.0
            );

            _context.ParkingLocations.AddRange(location1, location2);
            _context.SaveChanges();

            // Act
            var results = _repository.Get();

            // Assert
            results.Should().HaveCount(2);
            results.Should().Contain(pl => pl.Name == "Lviv City Center");
            results.Should().Contain(pl => pl.Name == "Odesa Port Parking");
        }

        [Fact]
        public void Get_WithFilter_ShouldReturnFilteredParkingLocations()
        {
            // Arrange
            var location1 = new ParkingLocation(
                "Dnipro Mall Parking",
                "Ukraine",
                "Dnipro",
                "49000",
                "Naberezhna Peremohy 1",
                50,
                35.0
            );

            var location2 = new ParkingLocation(
                "Kharkiv Arena",
                "Ukraine",
                "Kharkiv",
                "61000",
                "Plekhanivsʹka Street 100",
                200,
                60.0
            );

            var location3 = new ParkingLocation(
                "Poltava Center",
                "Ukraine",
                "Poltava",
                "36000",
                "Sobornist Street 10",
                75,
                38.0
            );

            _context.ParkingLocations.AddRange(location1, location2, location3);
            _context.SaveChanges();

            // Act
            var results = _repository.Get(pl => pl.AmountSlots > 60);

            // Assert
            results.Should().HaveCount(2);
            results.Should().Contain(pl => pl.Name == "Kharkiv Arena");
            results.Should().Contain(pl => pl.Name == "Poltava Center");
            results.Should().NotContain(pl => pl.Name == "Dnipro Mall Parking");
        }

        [Fact]
        public void Get_ShouldFilterByCity()
        {
            // Arrange
            var location1 = new ParkingLocation(
                "Podil Parking",
                "Ukraine",
                "Kyiv",
                "04070",
                "Sahaidachnoho Street 15",
                90,
                48.0
            );

            var location2 = new ParkingLocation(
                "Pechersk Parking",
                "Ukraine",
                "Kyiv",
                "01010",
                "Ivana Mazepy Street 3",
                110,
                52.0
            );

            var location3 = new ParkingLocation(
                "Zaporizhzhia Station",
                "Ukraine",
                "Zaporizhzhia",
                "69000",
                "Sobornyi Avenue 150",
                65,
                42.0
            );

            _context.ParkingLocations.AddRange(location1, location2, location3);
            _context.SaveChanges();

            // Act
            var results = _repository.Get(pl => pl.City == "Kyiv");

            // Assert
            results.Should().HaveCount(2);
            results.Should().Contain(pl => pl.Name == "Podil Parking");
            results.Should().Contain(pl => pl.Name == "Pechersk Parking");
            results.Should().NotContain(pl => pl.Name == "Zaporizhzhia Station");
        }

        [Fact]
        public void Get_ShouldIncludeVehicles()
        {
            // Arrange
            var parkingLocation = new ParkingLocation(
                "Shevchenko Park Parking",
                "Ukraine",
                "Kyiv",
                "01004",
                "Volodymyrska Street 40",
                100,
                45.0
            );

            var vehicle = new Vehicle("KA1122AA", "Volkswagen Passat");

            _context.ParkingLocations.Add(parkingLocation);
            _context.SaveChanges();

            vehicle.ParkingLocationId = parkingLocation.Id;
            _context.Vehicles.Add(vehicle);
            _context.SaveChanges();

            // Act
            var results = _repository.Get(pl => pl.Id == parkingLocation.Id);

            // Assert
            var result = results.FirstOrDefault();
            result.Should().NotBeNull();
            result.Vehicles.Should().HaveCount(1);
            result.Vehicles.First().LicensePlate.Should().Be("KA1122AA");
            result.Vehicles.First().Model.Should().Be("Volkswagen Passat");
        }

        [Fact]
        public void Remove_ShouldDeleteParkingLocationFromDatabase()
        {
            // Arrange
            var parkingLocation = new ParkingLocation(
                "Chernihiv Old Town",
                "Ukraine",
                "Chernihiv",
                "14000",
                "Prospekt Myru 25",
                40,
                30.0
            );

            _context.ParkingLocations.Add(parkingLocation);
            _context.SaveChanges();

            // Act
            _repository.Remove(parkingLocation);

            // Assert
            var result = _context.ParkingLocations.Find(parkingLocation.Id);
            result.Should().BeNull();
        }

        [Fact]
        public void Remove_ShouldSetVehicleParkingLocationToNull_WhenDeleteBehaviorIsSetNull()
        {
            // Arrange
            var parkingLocation = new ParkingLocation(
                "Vinnytsia Center",
                "Ukraine",
                "Vinnytsia",
                "21000",
                "Soborna Street 50",
                55,
                36.0
            );

            var vehicle = new Vehicle("AA9988BC", "Skoda Octavia");

            _context.ParkingLocations.Add(parkingLocation);
            _context.SaveChanges();

            vehicle.ParkingLocationId = parkingLocation.Id;
            _context.Vehicles.Add(vehicle);
            _context.SaveChanges();

            // Act
            _repository.Remove(parkingLocation);

            // Assert
            var orphanedVehicle = _context.Vehicles.Find(vehicle.Id);
            orphanedVehicle.Should().NotBeNull();
            orphanedVehicle.ParkingLocationId.Should().BeNull();
            orphanedVehicle.LicensePlate.Should().Be("AA9988BC");
        }

        [Fact]
        public void Get_ShouldFilterByTarifRate()
        {
            // Arrange
            var location1 = new ParkingLocation(
                "Ivano-Frankivsk Plaza",
                "Ukraine",
                "Ivano-Frankivsk",
                "76000",
                "Nezalezhnosti Street 2",
                70,
                35.0
            );

            var location2 = new ParkingLocation(
                "Obolon Shopping Mall",
                "Ukraine",
                "Kyiv",
                "04205",
                "Obolonsky Avenue 1",
                180,
                58.0
            );

            _context.ParkingLocations.AddRange(location1, location2);
            _context.SaveChanges();

            // Act
            var results = _repository.Get(pl => pl.TarifRate < 50.0);

            // Assert
            results.Should().HaveCount(1);
            results.First().Name.Should().Be("Ivano-Frankivsk Plaza");
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}