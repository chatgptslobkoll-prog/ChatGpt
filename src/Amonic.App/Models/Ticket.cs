namespace Amonic.App.Models
{
    public class Ticket
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public int ScheduleID { get; set; }
        public int CabinTypeID { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string PassportNumber { get; set; }
        public int PassportCountryID { get; set; }
        public string BookingReference { get; set; }
        public bool Confirmed { get; set; }
    }
}
