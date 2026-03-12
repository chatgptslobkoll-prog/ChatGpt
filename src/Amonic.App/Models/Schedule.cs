using System;

namespace Amonic.App.Models
{
    public class Schedule
    {
        public int ID { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public int AircraftID { get; set; }
        public int RouteID { get; set; }
        public decimal EconomyPrice { get; set; }
        public bool Confirmed { get; set; }
        public string FlightNumber { get; set; }
    }
}
