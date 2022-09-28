using System;

namespace ScooterRental
{
    public class RentedScooter
    {
        public string Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }       
        public decimal PricePerMinute { get; set; }

        public decimal TotalPrice { get; set; }

        public RentedScooter (string id, DateTime startTime, decimal pricePerMinute)
        {
            Id = id;
            StartTime = startTime;
            PricePerMinute = pricePerMinute;
        }
    }
}
