using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Data.SqlClient;
using System.Data.Common;

namespace FlightPlanner.DataLayer
{
    // Implement operations that affect several tables (e.g. deleting a flight affects also the Booking table)
    class FlightRepository
    {
        private BookingDataMapper bookingDataMapper;
        private FlightDataMapper flightDataMapper;
        // TODO: add other data mappers
        string ConnectionString { get; set; }

        public FlightRepository(string connectionString)
        {
            this.ConnectionString = connectionString;
            bookingDataMapper = new BookingDataMapper(this.ConnectionString);
            flightDataMapper = new FlightDataMapper(this.ConnectionString);
        }

        public void ReadAllFlightsAndCustomersCrossed()
        {
            using (DbConnection databaseConnection = new SqlConnection(this.ConnectionString))
            {
                IDbCommand command = databaseConnection.CreateCommand();

                // FULL OUTER JOIN sorgt dafür, dass:
                // 1. Kunden ohne Buchung/Flug aufgereiht werden
                // 2. Flüge ohne Buchung/Kunde aufgereiht werden
                command.CommandText = @"
                    SELECT 
                        c.Id AS CustomerId, c.FirstName, c.LastName, 
                        f.Id AS FlightId, f.FlightNumber, b.Seats
                    FROM Customer c
                    FULL OUTER JOIN Booking b ON c.Id = b.CustomerId
                    FULL OUTER JOIN Flight f ON b.FlightId = f.Id;";

                Console.WriteLine("Executing: " + command.CommandText);
                databaseConnection.Open();

                using (IDataReader reader = command.ExecuteReader())
                {
                    Console.WriteLine("\n--- FLUG & KUNDEN ÜBERSICHT ---");
                    while (reader.Read())
                    {
                        // Werte auslesen (und auf NULL prüfen, da FULL JOIN leere Felder erzeugt)
                        string customerId = reader["CustomerId"] == DBNull.Value ? "Kein Kunde" : reader["CustomerId"].ToString();
                        string customerName = reader["LastName"] == DBNull.Value ? "" : $"{reader["FirstName"]} {reader["LastName"]}";
                        string flightId = reader["FlightId"] == DBNull.Value ? "Kein Flug" : reader["FlightId"].ToString();
                        string flightNumber = reader["FlightNumber"] == DBNull.Value ? "Ungebucht" : reader["FlightNumber"].ToString();

                        Console.WriteLine($"Kunde: [{customerId}] {customerName.PadRight(20)} | Flug: [{flightId}] {flightNumber}");
                    }
                    Console.WriteLine("---------------------------------\n");
                }
            }
        }

        public void DeleteFlight(int id)
        {
            int rowCount = Int32.MinValue;
            try
            {
                // FK_Booking_Flight uses "on delete no action"
                // FK_PilotRoster_Flight uses "ON DELETE CASCADE"
                rowCount = bookingDataMapper.DeleteByFlightId(id);
                rowCount = flightDataMapper.Delete(id);
            }
            catch (DbException dbEx) // TODO: review and improve exception handling
            {
                // TODO: log to log file
                throw new InvalidOperationException("Flight could not be deleted!", dbEx);
            }
            catch (Exception)
            {
                // TODO: log to log file
                throw;
            }

            if (rowCount < 1)
            {
                throw new InvalidOperationException("The specified flight could not be deleted.");
            }
        }

    }
}
