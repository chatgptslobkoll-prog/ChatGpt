namespace Amonic.App.Models
{
    public class Route
    {
        public int ID { get; set; }
        public int DepartureAirportID { get; set; }
        public int ArrivalAirportID { get; set; }
        public int Distance { get; set; }
        public int FlightTime { get; set; }
    }
}
