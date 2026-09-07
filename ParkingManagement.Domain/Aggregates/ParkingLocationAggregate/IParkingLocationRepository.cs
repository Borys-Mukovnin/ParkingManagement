using System.Linq.Expressions;
using ParkingManagement.Domain.Aggregates.ParkingLocationAggregate;

namespace ParkingManagement.Infrastructure
{
    public interface IParkingLocationRepository
    {
        ParkingLocation GetById(Guid id);
        IEnumerable<ParkingLocation> Get(Expression<Func<ParkingLocation, bool>> filter = null);
        void Add(ParkingLocation parkingLocation);
        void Remove(ParkingLocation parkingLocation);
    }
}
