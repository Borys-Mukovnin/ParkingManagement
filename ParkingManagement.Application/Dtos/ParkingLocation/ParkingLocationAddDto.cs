namespace ParkingManagement.Application.Dtos.ParkingLocation
{
    public readonly record struct ParkingLocationAddDto(string name, string country, string city, string zipcode, string street, int amountSlots, double tarifRate);
}
