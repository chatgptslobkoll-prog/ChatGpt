using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using Amonic.App.Data;
using Amonic.App.ViewModels;
using Amonic.App.Models;

namespace Amonic.App.Services
{
    public class AppRepository
    {
        public List<OfficeItem> GetOffices()
        {
            var list = new List<OfficeItem>();
            using (var cn = Db.OpenConnection())
            using (var cmd = new SqlCommand("SELECT ID, Title FROM Offices ORDER BY Title", cn))
            using (var rd = cmd.ExecuteReader())
            {
                while (rd.Read())
                {
                    list.Add(new OfficeItem { ID = (int)rd["ID"], Title = rd["Title"].ToString() });
                }
            }
            return list;
        }

        public List<UserListItem> GetUsers(int? officeId)
        {
            var list = new List<UserListItem>();
            using (var cn = Db.OpenConnection())
            using (var cmd = cn.CreateCommand())
            {
                cmd.CommandText = @"SELECT u.ID, u.FirstName, u.LastName, u.Birthdate, r.Title AS RoleTitle, u.Email,
ISNULL(o.Title, '-') AS OfficeTitle, ISNULL(u.Active, 0) AS Active
FROM Users u
JOIN Roles r ON r.ID = u.RoleID
LEFT JOIN Offices o ON o.ID = u.OfficeID
WHERE (@OfficeID IS NULL OR u.OfficeID = @OfficeID)
ORDER BY u.LastName, u.FirstName";
                cmd.Parameters.AddWithValue("@OfficeID", (object)officeId ?? DBNull.Value);
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        var birth = rd["Birthdate"] == DBNull.Value ? (DateTime?)null : (DateTime)rd["Birthdate"];
                        list.Add(new UserListItem
                        {
                            ID = (int)rd["ID"],
                            FirstName = rd["FirstName"].ToString(),
                            LastName = rd["LastName"].ToString(),
                            Age = birth.HasValue ? CalculateAge(birth.Value) : (int?)null,
                            Role = rd["RoleTitle"].ToString(),
                            Email = rd["Email"].ToString(),
                            Office = rd["OfficeTitle"].ToString(),
                            Active = Convert.ToBoolean(rd["Active"])
                        });
                    }
                }
            }
            return list;
        }

        public void ToggleUserActive(int userId)
        {
            using (var cn = Db.OpenConnection())
            using (var cmd = new SqlCommand("UPDATE Users SET Active = CASE WHEN ISNULL(Active,0)=1 THEN 0 ELSE 1 END WHERE ID=@ID", cn))
            {
                cmd.Parameters.AddWithValue("@ID", userId);
                cmd.ExecuteNonQuery();
            }
        }

        public void ChangeUserRole(int userId, int roleId)
        {
            using (var cn = Db.OpenConnection())
            using (var cmd = new SqlCommand("UPDATE Users SET RoleID=@RoleID WHERE ID=@ID", cn))
            {
                cmd.Parameters.AddWithValue("@ID", userId);
                cmd.Parameters.AddWithValue("@RoleID", roleId);
                cmd.ExecuteNonQuery();
            }
        }

        public int AddUser(string email, string password, string firstName, string lastName, int officeId, DateTime birthdate)
        {
            using (var cn = Db.OpenConnection())
            using (var cmd = cn.CreateCommand())
            {
                cmd.CommandText = @"DECLARE @NewID INT = ISNULL((SELECT MAX(ID)+1 FROM Users),1);
INSERT INTO Users(ID, RoleID, Email, Password, FirstName, LastName, OfficeID, Birthdate, Active)
VALUES (@NewID, 2, @Email, @Password, @FirstName, @LastName, @OfficeID, @Birthdate, 1);
SELECT @NewID;";
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Password", password);
                cmd.Parameters.AddWithValue("@FirstName", firstName);
                cmd.Parameters.AddWithValue("@LastName", lastName);
                cmd.Parameters.AddWithValue("@OfficeID", officeId);
                cmd.Parameters.AddWithValue("@Birthdate", birthdate.Date);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public int CreateActivityLog(int userId)
        {
            using (var cn = Db.OpenConnection())
            using (var cmd = new SqlCommand("INSERT INTO UserActivityLogs(UserID, LoginAt) VALUES(@UserID, SYSDATETIME()); SELECT SCOPE_IDENTITY();", cn))
            {
                cmd.Parameters.AddWithValue("@UserID", userId);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public void CloseActivityLog(int logId)
        {
            using (var cn = Db.OpenConnection())
            using (var cmd = new SqlCommand("UPDATE UserActivityLogs SET LogoutAt=SYSDATETIME() WHERE ID=@ID AND LogoutAt IS NULL", cn))
            {
                cmd.Parameters.AddWithValue("@ID", logId);
                cmd.ExecuteNonQuery();
            }
        }

        public (string fullName, string timeSpent, int crashes, List<UserActivityItem> logs) GetUserDashboard(int userId)
        {
            string fullName;
            using (var cn = Db.OpenConnection())
            using (var cmd = new SqlCommand("SELECT FirstName + ' ' + LastName FROM Users WHERE ID=@ID", cn))
            {
                cmd.Parameters.AddWithValue("@ID", userId);
                fullName = Convert.ToString(cmd.ExecuteScalar()) ?? "User";
            }

            var logs = new List<UserActivityItem>();
            TimeSpan total = TimeSpan.Zero;
            int crashes = 0;
            using (var cn = Db.OpenConnection())
            using (var cmd = cn.CreateCommand())
            {
                cmd.CommandText = @"SELECT LoginAt, LogoutAt, CrashReason
FROM UserActivityLogs
WHERE UserID=@UserID AND LoginAt >= DATEADD(DAY,-30,SYSDATETIME())
ORDER BY LoginAt DESC";
                cmd.Parameters.AddWithValue("@UserID", userId);
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        var login = (DateTime)rd["LoginAt"];
                        var logout = rd["LogoutAt"] == DBNull.Value ? (DateTime?)null : (DateTime)rd["LogoutAt"];
                        var crash = rd["CrashReason"] == DBNull.Value ? string.Empty : rd["CrashReason"].ToString();
                        if (logout.HasValue)
                        {
                            total += logout.Value - login;
                        }
                        if (!string.IsNullOrWhiteSpace(crash))
                        {
                            crashes++;
                        }

                        logs.Add(new UserActivityItem
                        {
                            LoginAt = login,
                            LogoutAt = logout,
                            Duration = logout.HasValue ? (logout.Value - login).ToString(@"hh\:mm\:ss") : "-",
                            CrashReason = crash
                        });
                    }
                }
            }

            return (fullName, total.ToString(@"hh\:mm\:ss"), crashes, logs.Where(l => l.LogoutAt.HasValue).ToList());
        }


        public List<Country> GetCountries()
        {
            var list = new List<Country>();
            using (var cn = Db.OpenConnection())
            using (var cmd = new SqlCommand("SELECT ID, Name FROM Countries ORDER BY Name", cn))
            using (var rd = cmd.ExecuteReader())
            {
                while (rd.Read())
                {
                    list.Add(new Country { ID = (int)rd["ID"], Name = rd["Name"].ToString() });
                }
            }
            return list;
        }

        public List<AirportItem> GetAirports()
        {
            var list = new List<AirportItem>();
            using (var cn = Db.OpenConnection())
            using (var cmd = new SqlCommand("SELECT ID, IATACode, Name FROM Airports ORDER BY IATACode", cn))
            using (var rd = cmd.ExecuteReader())
            {
                while (rd.Read())
                {
                    list.Add(new AirportItem { ID = (int)rd["ID"], IATACode = rd["IATACode"].ToString(), Name = rd["Name"].ToString() });
                }
            }
            return list;
        }

        public List<ScheduleListItem> SearchSchedules(int? fromAirportId, int? toAirportId, DateTime? date, string flightNo, string sortBy)
        {
            var list = new List<ScheduleListItem>();
            using (var cn = Db.OpenConnection())
            using (var cmd = cn.CreateCommand())
            {
                cmd.CommandText = @"SELECT s.ID, s.[Date], s.[Time], da.IATACode AS [FromCode], aa.IATACode AS [ToCode], s.FlightNumber,
a.Name AS Aircraft, s.EconomyPrice, s.Confirmed
FROM Schedules s
JOIN Routes r ON r.ID = s.RouteID
JOIN Airports da ON da.ID = r.DepartureAirportID
JOIN Airports aa ON aa.ID = r.ArrivalAirportID
JOIN Aircrafts a ON a.ID = s.AircraftID
WHERE (@FromID IS NULL OR r.DepartureAirportID = @FromID)
  AND (@ToID IS NULL OR r.ArrivalAirportID = @ToID)
  AND (@Date IS NULL OR s.[Date] = @Date)
  AND (@FlightNo = '' OR s.FlightNumber LIKE '%' + @FlightNo + '%')";
                cmd.Parameters.AddWithValue("@FromID", (object)fromAirportId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ToID", (object)toAirportId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Date", (object)date ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FlightNo", flightNo ?? string.Empty);

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        var eco = Convert.ToDecimal(rd["EconomyPrice"]);
                        var business = Math.Floor(eco * 1.35m);
                        var first = Math.Floor(business * 1.30m);
                        list.Add(new ScheduleListItem
                        {
                            ID = (int)rd["ID"],
                            Date = (DateTime)rd["Date"],
                            Time = (TimeSpan)rd["Time"],
                            From = rd["FromCode"].ToString(),
                            To = rd["ToCode"].ToString(),
                            FlightNumber = rd["FlightNumber"].ToString(),
                            Aircraft = rd["Aircraft"].ToString(),
                            Economy = eco,
                            Business = (decimal)business,
                            First = (decimal)first,
                            Confirmed = Convert.ToBoolean(rd["Confirmed"])
                        });
                    }
                }
            }

            switch (sortBy)
            {
                case "Economy Price":
                    return list.OrderBy(x => x.Economy).ToList();
                case "Confirmed":
                    return list.OrderByDescending(x => x.Confirmed).ThenBy(x => x.Date).ThenBy(x => x.Time).ToList();
                default:
                    return list.OrderBy(x => x.Date).ThenBy(x => x.Time).ToList();
            }
        }

        public void ToggleSchedule(int id)
        {
            using (var cn = Db.OpenConnection())
            using (var cmd = new SqlCommand("UPDATE Schedules SET Confirmed = CASE WHEN Confirmed=1 THEN 0 ELSE 1 END WHERE ID=@ID", cn))
            {
                cmd.Parameters.AddWithValue("@ID", id);
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateSchedule(int id, DateTime date, TimeSpan time, decimal economyPrice)
        {
            using (var cn = Db.OpenConnection())
            using (var cmd = new SqlCommand("UPDATE Schedules SET [Date]=@Date, [Time]=@Time, EconomyPrice=@Price WHERE ID=@ID", cn))
            {
                cmd.Parameters.AddWithValue("@ID", id);
                cmd.Parameters.AddWithValue("@Date", date.Date);
                cmd.Parameters.AddWithValue("@Time", time);
                cmd.Parameters.AddWithValue("@Price", economyPrice);
                cmd.ExecuteNonQuery();
            }
        }

        public ImportResultDto ImportSchedules(string path)
        {
            var result = new ImportResultDto();
            if (!File.Exists(path)) return result;

            var processedKeys = new HashSet<string>();
            foreach (var line in File.ReadLines(path).Skip(1))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                var p = line.Split(',');
                if (p.Length < 9)
                {
                    result.Invalid++;
                    continue;
                }

                var action = p[0].Trim();
                if (!DateTime.TryParse(p[1], out var date) || !TimeSpan.TryParse(p[2], out var time)) { result.Invalid++; continue; }
                var flightNo = p[3].Trim();
                var fromIata = p[4].Trim();
                var toIata = p[5].Trim();
                var aircraftName = p[6].Trim();
                if (!decimal.TryParse(p[7], NumberStyles.Any, CultureInfo.InvariantCulture, out var price)) { result.Invalid++; continue; }
                var confirmed = p[8].Trim().Equals("OK", StringComparison.OrdinalIgnoreCase);

                var key = flightNo + "|" + date.ToString("yyyy-MM-dd");
                if (!processedKeys.Add(key))
                {
                    result.Duplicates++;
                    continue;
                }

                using (var cn = Db.OpenConnection())
                using (var cmd = cn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT TOP 1 s.ID
FROM Schedules s
WHERE s.FlightNumber=@FlightNo AND s.[Date]=@Date";
                    cmd.Parameters.AddWithValue("@FlightNo", flightNo);
                    cmd.Parameters.AddWithValue("@Date", date.Date);
                    var existingObj = cmd.ExecuteScalar();

                    if (action.Equals("EDIT", StringComparison.OrdinalIgnoreCase))
                    {
                        if (existingObj == null)
                        {
                            result.Invalid++;
                            continue;
                        }

                        using (var u = new SqlCommand("UPDATE Schedules SET [Time]=@Time, EconomyPrice=@Price, Confirmed=@Confirmed WHERE ID=@ID", cn))
                        {
                            u.Parameters.AddWithValue("@ID", Convert.ToInt32(existingObj));
                            u.Parameters.AddWithValue("@Time", time);
                            u.Parameters.AddWithValue("@Price", price);
                            u.Parameters.AddWithValue("@Confirmed", confirmed);
                            u.ExecuteNonQuery();
                            result.Updated++;
                        }
                    }
                    else if (action.Equals("ADD", StringComparison.OrdinalIgnoreCase))
                    {
                        if (existingObj != null)
                        {
                            result.Duplicates++;
                            continue;
                        }

                        var routeId = ResolveRouteId(cn, fromIata, toIata);
                        var aircraftId = ResolveAircraftId(cn, aircraftName);
                        if (!routeId.HasValue || !aircraftId.HasValue)
                        {
                            result.Invalid++;
                            continue;
                        }

                        using (var i = new SqlCommand("INSERT INTO Schedules([Date],[Time],AircraftID,RouteID,EconomyPrice,Confirmed,FlightNumber) VALUES(@Date,@Time,@Aircraft,@Route,@Price,@Confirmed,@FlightNo)", cn))
                        {
                            i.Parameters.AddWithValue("@Date", date.Date);
                            i.Parameters.AddWithValue("@Time", time);
                            i.Parameters.AddWithValue("@Aircraft", aircraftId.Value);
                            i.Parameters.AddWithValue("@Route", routeId.Value);
                            i.Parameters.AddWithValue("@Price", price);
                            i.Parameters.AddWithValue("@Confirmed", confirmed);
                            i.Parameters.AddWithValue("@FlightNo", flightNo);
                            i.ExecuteNonQuery();
                            result.Added++;
                        }
                    }
                    else
                    {
                        result.Invalid++;
                    }
                }
            }

            return result;
        }

        public List<FlightOptionItem> SearchFlightOptions(int fromId, int toId, DateTime date, bool aroundDays, int cabinTypeId)
        {
            var start = aroundDays ? date.AddDays(-3).Date : date.Date;
            var end = aroundDays ? date.AddDays(3).Date : date.Date;
            var list = new List<FlightOptionItem>();
            using (var cn = Db.OpenConnection())
            using (var cmd = cn.CreateCommand())
            {
                cmd.CommandText = @"SELECT s.ID, s.[Date], s.[Time], da.IATACode AS [FromCode], aa.IATACode AS [ToCode], s.FlightNumber,
s.EconomyPrice, a.TotalSeats, a.EconomySeats, a.BusinessSeats
FROM Schedules s
JOIN Routes r ON r.ID=s.RouteID
JOIN Airports da ON da.ID=r.DepartureAirportID
JOIN Airports aa ON aa.ID=r.ArrivalAirportID
JOIN Aircrafts a ON a.ID=s.AircraftID
WHERE r.DepartureAirportID=@FromID AND r.ArrivalAirportID=@ToID AND s.Confirmed=1
AND s.[Date] BETWEEN @StartDate AND @EndDate
ORDER BY s.[Date], s.[Time]";
                cmd.Parameters.AddWithValue("@FromID", fromId);
                cmd.Parameters.AddWithValue("@ToID", toId);
                cmd.Parameters.AddWithValue("@StartDate", start);
                cmd.Parameters.AddWithValue("@EndDate", end);

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        var scheduleId = (int)rd["ID"];
                        var eco = Convert.ToDecimal(rd["EconomyPrice"]);
                        var price = ComputeCabinPrice(eco, cabinTypeId);
                        list.Add(new FlightOptionItem
                        {
                            ScheduleID = scheduleId,
                            From = rd["FromCode"].ToString(),
                            To = rd["ToCode"].ToString(),
                            Date = (DateTime)rd["Date"],
                            Time = (TimeSpan)rd["Time"],
                            Flights = rd["FlightNumber"].ToString(),
                            Price = price,
                            FreeSeats = GetFreeSeats(cn, scheduleId, cabinTypeId, (int)rd["TotalSeats"], (int)rd["EconomySeats"], (int)rd["BusinessSeats"])
                        });
                    }
                }
            }
            return list;
        }

        public IssueResult IssueTickets(int userId, List<int> scheduleIds, int cabinTypeId, List<PassengerInput> passengers)
        {
            var refCode = GenerateBookingReference();
            var tickets = new List<TicketSummaryItem>();

            using (var cn = Db.OpenConnection())
            {
                foreach (var scheduleId in scheduleIds)
                {
                    var flightNo = GetFlightNo(cn, scheduleId);
                    var price = GetCabinPrice(cn, scheduleId, cabinTypeId);
                    foreach (var p in passengers)
                    {
                        using (var cmd = cn.CreateCommand())
                        {
                            cmd.CommandText = @"INSERT INTO Tickets(UserID, ScheduleID, CabinTypeID, Firstname, Lastname, Email, Phone, PassportNumber, PassportCountryID, BookingReference, Confirmed)
VALUES (@UserID,@ScheduleID,@CabinTypeID,@Firstname,@Lastname,@Email,@Phone,@PassportNumber,@PassportCountryID,@BookingReference,1)";
                            cmd.Parameters.AddWithValue("@UserID", userId);
                            cmd.Parameters.AddWithValue("@ScheduleID", scheduleId);
                            cmd.Parameters.AddWithValue("@CabinTypeID", cabinTypeId);
                            cmd.Parameters.AddWithValue("@Firstname", p.FirstName);
                            cmd.Parameters.AddWithValue("@Lastname", p.LastName);
                            cmd.Parameters.AddWithValue("@Email", (object)p.Email ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@Phone", p.Phone);
                            cmd.Parameters.AddWithValue("@PassportNumber", p.PassportNumber);
                            cmd.Parameters.AddWithValue("@PassportCountryID", p.PassportCountryID);
                            cmd.Parameters.AddWithValue("@BookingReference", refCode);
                            cmd.ExecuteNonQuery();
                        }

                        tickets.Add(new TicketSummaryItem
                        {
                            Passenger = p.FirstName + " " + p.LastName,
                            Flight = flightNo,
                            Cabin = cabinTypeId == 1 ? "Economy" : cabinTypeId == 2 ? "Business" : "First",
                            Price = price
                        });
                    }
                }
            }

            return new IssueResult { BookingReference = refCode, Tickets = tickets };
        }

        private static int CalculateAge(DateTime birth)
        {
            var age = DateTime.Today.Year - birth.Year;
            if (birth > DateTime.Today.AddYears(-age)) age--;
            return age;
        }

        private static int? ResolveRouteId(SqlConnection cn, string from, string to)
        {
            using (var cmd = new SqlCommand(@"SELECT TOP 1 r.ID
FROM Routes r
JOIN Airports da ON da.ID=r.DepartureAirportID
JOIN Airports aa ON aa.ID=r.ArrivalAirportID
WHERE da.IATACode=@From AND aa.IATACode=@To", cn))
            {
                cmd.Parameters.AddWithValue("@From", from);
                cmd.Parameters.AddWithValue("@To", to);
                var obj = cmd.ExecuteScalar();
                return obj == null ? (int?)null : Convert.ToInt32(obj);
            }
        }

        private static int? ResolveAircraftId(SqlConnection cn, string aircraftName)
        {
            using (var cmd = new SqlCommand("SELECT TOP 1 ID FROM Aircrafts WHERE Name=@Name OR MakeModel=@Name", cn))
            {
                cmd.Parameters.AddWithValue("@Name", aircraftName);
                var obj = cmd.ExecuteScalar();
                return obj == null ? (int?)null : Convert.ToInt32(obj);
            }
        }

        private static decimal ComputeCabinPrice(decimal economy, int cabinTypeId)
        {
            if (cabinTypeId == 2)
            {
                return (decimal)Math.Floor(economy * 1.35m);
            }
            if (cabinTypeId == 3)
            {
                var business = Math.Floor(economy * 1.35m);
                return (decimal)Math.Floor((decimal)business * 1.30m);
            }
            return economy;
        }

        private int GetFreeSeats(SqlConnection cn, int scheduleId, int cabinTypeId, int total, int economy, int business)
        {
            var cap = cabinTypeId == 1 ? economy : cabinTypeId == 2 ? business : Math.Max(total - economy - business, 0);
            using (var cmd = new SqlCommand("SELECT COUNT(1) FROM Tickets WHERE ScheduleID=@ScheduleID AND CabinTypeID=@CabinTypeID", cn))
            {
                cmd.Parameters.AddWithValue("@ScheduleID", scheduleId);
                cmd.Parameters.AddWithValue("@CabinTypeID", cabinTypeId);
                var sold = Convert.ToInt32(cmd.ExecuteScalar());
                return Math.Max(cap - sold, 0);
            }
        }

        private string GetFlightNo(SqlConnection cn, int scheduleId)
        {
            using (var cmd = new SqlCommand("SELECT FlightNumber FROM Schedules WHERE ID=@ID", cn))
            {
                cmd.Parameters.AddWithValue("@ID", scheduleId);
                return Convert.ToString(cmd.ExecuteScalar()) ?? "-";
            }
        }

        private decimal GetCabinPrice(SqlConnection cn, int scheduleId, int cabinTypeId)
        {
            using (var cmd = new SqlCommand("SELECT EconomyPrice FROM Schedules WHERE ID=@ID", cn))
            {
                cmd.Parameters.AddWithValue("@ID", scheduleId);
                var eco = Convert.ToDecimal(cmd.ExecuteScalar());
                return ComputeCabinPrice(eco, cabinTypeId);
            }
        }

        private string GenerateBookingReference()
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            var random = new Random();

            while (true)
            {
                var code = new string(Enumerable.Range(0, 6).Select(_ => chars[random.Next(chars.Length)]).ToArray());
                using (var cn = Db.OpenConnection())
                using (var cmd = new SqlCommand("SELECT COUNT(1) FROM Tickets WHERE BookingReference=@Ref", cn))
                {
                    cmd.Parameters.AddWithValue("@Ref", code);
                    if (Convert.ToInt32(cmd.ExecuteScalar()) == 0)
                    {
                        return code;
                    }
                }
            }
        }
    }
}
