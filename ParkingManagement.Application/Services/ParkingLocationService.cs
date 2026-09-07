using System.Linq.Expressions;
using ParkingManagement.Application.Dtos.ParkingLocation;
using ParkingManagement.Domain.Aggregates.ParkingLocationAggregate;
using ParkingManagement.Infrastructure;

namespace ParkingManagement.Application.Services
{
    public class ParkingLocationService : IParkingLocationService
    {
        private readonly IParkingLocationRepository parkingLocationRepository;

        public ParkingLocationService(IParkingLocationRepository parkingLocationRepository)
        {
            this.parkingLocationRepository = parkingLocationRepository;
        }

        public IEnumerable<ParkingLocationDto> GetParkingLocations(Expression<Func<ParkingLocation, bool>> filter = null)
        {
            return this.parkingLocationRepository.Get(filter).Select(pl => pl.ToDto());
        }

        public ParkingLocationDto GetParkingLocationById(Guid id)
        {
            return this.parkingLocationRepository.GetById(id).ToDto();
        }

        public void AddParkingLocation(ParkingLocationAddDto dto)
        {
            this.parkingLocationRepository.Add(dto.ToEntity());
        }

        public void RemoveParkingLocation(Guid id)
        {
            var parkingLocation = this.parkingLocationRepository.GetById(id);
            if (parkingLocation != null)
            {
                this.parkingLocationRepository.Remove(parkingLocation);
            }
        }
    }
}
