using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ParkingManagement.Domain.Aggregates.ParkingLocationAggregate;

namespace ParkingManagement.Infrastructure
{
    public class ParkingLocationRepository : IParkingLocationRepository
    {
        private readonly ParkingLocationContext context;
        public ParkingLocationRepository(ParkingLocationContext context)
        {
            this.context = context;
        }

        public ParkingLocation GetById(Guid id)
        {
            var t = context.ParkingLocations
                          .Include(pl => pl.Vehicles)
                          .FirstOrDefault(pl => pl.Id == id);
            return t;
        }

        public IEnumerable<ParkingLocation> Get(Expression<Func<ParkingLocation, bool>> filter = null)
        {
            return context.ParkingLocations
                          .Where(filter ?? (pl => true))
                          .Include(pl => pl.Vehicles)
                          .ToList();
        }

        public void Add(ParkingLocation parkingLocation)
        {
            context.ParkingLocations.Add(parkingLocation);
            context.SaveChanges();
        }

        public void Remove(ParkingLocation parkingLocation)
        {
            context.ParkingLocations.Remove(parkingLocation);
            context.SaveChanges();
        }
    }
}
