using ParkingManagement.Application.Dtos.Vehicle;

namespace ParkingManagement.Application.Dtos.ParkingLocation
{
    public readonly record struct ParkingLocationDto(Guid id, string name, string country, string city, string zipcode, string street, int amountSlots, double TarifRate, IEnumerable<VehicleDto> vehicles);
}
