namespace ParkingManagement.Domain.Aggregates.ParkingLocationAggregate
{
    public class Vehicle
    {
        public Guid Id { get; set; }
        public string LicensePlate { get; set; }
        public string Model { get; set; }
        public DateTime LastEntered { get; set; }


        /* Navigation property */
        public Guid? ParkingLocationId { get; set; }
        public ParkingLocation ParkingLocation { get; set; }

        /* An empty constructor is required by Entity Framework */
        private Vehicle() { }

        public Vehicle(string licensePlate, string model)
        {
            this.Id = Guid.NewGuid();
            this.LicensePlate = licensePlate;
            this.Model = model;
        }


        public double CalculateParkingFee(DateTime exitTime, double tarifRate)
        {
            TimeSpan duration = exitTime - LastEntered;
            double hoursParked = Math.Ceiling(duration.TotalHours);
            return hoursParked * tarifRate;
        }
    }
}
