namespace ParkingManagement.Application.Dtos.Vehicle
{
    public readonly record struct VehicleDto(Guid id, string licensePlate, string model, DateTime LastEntered);
}
