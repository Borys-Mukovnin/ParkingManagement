using System.Linq.Expressions;
using ParkingManagement.Application.Dtos.ParkingLocation;
using ParkingManagement.Domain.Aggregates.ParkingLocationAggregate;

namespace ParkingManagement.Application.Services
{
    public interface IParkingLocationService
    {
        IEnumerable<ParkingLocationDto> GetParkingLocations(Expression<Func<ParkingLocation, bool>> filter = null);
        ParkingLocationDto GetParkingLocationById(Guid id);
        void AddParkingLocation(ParkingLocationAddDto dto);
        void RemoveParkingLocation(Guid id);
    }
}
