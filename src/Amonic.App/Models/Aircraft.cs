namespace Amonic.App.Models
{
    public class Aircraft
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string MakeModel { get; set; }
        public int TotalSeats { get; set; }
        public int EconomySeats { get; set; }
        public int BusinessSeats { get; set; }
    }
}
