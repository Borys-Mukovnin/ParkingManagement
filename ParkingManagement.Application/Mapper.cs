using ParkingManagement.Application.Dtos.ParkingLocation;
using ParkingManagement.Application.Dtos.Vehicle;
using ParkingManagement.Domain.Aggregates.ParkingLocationAggregate;

namespace ParkingManagement.Application
{
    public static class Mapper
    {
        public static ParkingLocation ToEntity(this ParkingLocationAddDto dto)
        {
            return new ParkingLocation(
            dto.name,
            dto.country,
            dto.city,
            dto.zipcode,
            dto.street,
            dto.amountSlots,
            dto.tarifRate
        );
        }

        public static ParkingLocationDto ToDto(this ParkingLocation parkingLocation)
        {
            return new ParkingLocationDto(
            parkingLocation.Id,
            parkingLocation.Name,
            parkingLocation.Country,
            parkingLocation.City,
            parkingLocation.ZipCode,
            parkingLocation.Street,
            parkingLocation.AmountSlots,
            parkingLocation.TarifRate,
            parkingLocation.Vehicles.Select(v => v.ToDto())
            );
        }


        public static VehicleDto ToDto(this Vehicle vehicle)
        {
            return new VehicleDto(
            vehicle.Id,
            vehicle.LicensePlate,
            vehicle.Model,
            vehicle.LastEntered
            );
        }

        public static Vehicle ToEntity(this VehicleAddDto dto)
        {
            return new Vehicle(
            dto.licensePlate,
            dto.model
        );
        }
    }
}
