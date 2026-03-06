using System;
using System.Collections.Generic;

namespace Amonic.App.ViewModels
{
    public class OfficeItem
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public override string ToString() => Title;
    }

    public class UserListItem
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? Age { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
        public string Office { get; set; }
        public bool Active { get; set; }
    }

    public class UserActivityItem
    {
        public DateTime LoginAt { get; set; }
        public DateTime? LogoutAt { get; set; }
        public string Duration { get; set; }
        public string CrashReason { get; set; }
    }

    public class AirportItem
    {
        public int ID { get; set; }
        public string IATACode { get; set; }
        public string Name { get; set; }
        public override string ToString() => IATACode;
    }

    public class ScheduleListItem
    {
        public int ID { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string FlightNumber { get; set; }
        public string Aircraft { get; set; }
        public decimal Economy { get; set; }
        public decimal Business { get; set; }
        public decimal First { get; set; }
        public bool Confirmed { get; set; }
    }

    public class ImportResultDto
    {
        public int Added { get; set; }
        public int Updated { get; set; }
        public int Duplicates { get; set; }
        public int Invalid { get; set; }
    }

    public class FlightOptionItem
    {
        public int ScheduleID { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public string Flights { get; set; }
        public decimal Price { get; set; }
        public int FreeSeats { get; set; }
    }

    public class PassengerInput
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? Birthdate { get; set; }
        public string PassportNumber { get; set; }
        public int PassportCountryID { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }

    public class TicketSummaryItem
    {
        public string Passenger { get; set; }
        public string Flight { get; set; }
        public string Cabin { get; set; }
        public decimal Price { get; set; }
    }

    public class IssueResult
    {
        public string BookingReference { get; set; }
        public List<TicketSummaryItem> Tickets { get; set; }
    }
}
