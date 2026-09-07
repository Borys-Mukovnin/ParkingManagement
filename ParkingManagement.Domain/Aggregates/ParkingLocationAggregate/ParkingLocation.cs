namespace ParkingManagement.Domain.Aggregates.ParkingLocationAggregate
{
    public class ParkingLocation
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string Street { get; set; }
        public int AmountSlots { get; set; }
        public double TarifRate { get; set; }
        public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();

        /* An empty constructor is required by Entity Framework */
        private ParkingLocation() { }

        public ParkingLocation(string name, string country, string city, string zipcode, string street, int amountSlots, double tarifRate)
        {
            this.Id = Guid.NewGuid();
            this.Name = name;
            this.Country = country;
            this.City = city;
            this.ZipCode = zipcode;
            this.Street = street;
            this.AmountSlots = amountSlots;
            this.TarifRate = tarifRate;
        }


        public void LetVehicleIn(Vehicle vehicle)
        {
            if (Vehicles.Count() >= AmountSlots)
            {
                throw new InvalidOperationException("No available parking slots.");
            }

            vehicle.LastEntered = DateTime.Now;

            Vehicles.Add(vehicle);
        }

        public double LetVehicleOut(Vehicle vehicle)
        {
            if (!Vehicles.Contains(vehicle))
            {
                throw new InvalidOperationException("Vehicle not found in this parking location.");
            }

            var exitTime = new DateTime(
                DateTime.Now.Year,
                DateTime.Now.Month,
                DateTime.Now.Day,
                DateTime.Now.Hour,
                DateTime.Now.Minute,
                0  // seconds = 0
);
            double fee = vehicle.CalculateParkingFee(exitTime, TarifRate);

            Vehicles.Remove(vehicle);

            return fee;
        }
    }
}
