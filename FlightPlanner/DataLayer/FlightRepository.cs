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

        /// <summary>
        /// Abfrage mit FULL OUTER JOIN, die:
        /// 1. Alle Flüge mit ihren Buchungen und Kunden anzeigt
        /// 2. Flüge OHNE Buchungen anzeigt
        /// 3. Kunden OHNE Buchungen anzeigt
        /// Diese Abfrage betrifft mehrere Tabellen: Flight, Booking, Customer
        /// </summary>
        public void ReadAllFlightsAndCustomersCrossed()
        {
            using (DbConnection databaseConnection = new SqlConnection(this.ConnectionString))
            {
                IDbCommand command = databaseConnection.CreateCommand();

                // FULL OUTER JOIN sorgt dafür, dass:
                // 1. Alle Flüge mit Buchungen und Kunden angezeigt werden
                // 2. Flüge ohne Buchung/Kunden angezeigt werden
                // 3. Kunden ohne Buchung/Flug angezeigt werden
                command.CommandText = @"
                    SELECT 
                        c.Id AS CustomerId, 
                        c.FirstName,
                        c.LastName, 
                        f.Id AS FlightId, 
                        f.Departure,
                        f.Destination,
                        b.Seats
                    FROM Customer c
                    FULL OUTER JOIN Booking b ON c.Id = b.CustomerId
                    FULL OUTER JOIN Flight f ON b.FlightId = f.Id
                    ORDER BY c.Id, f.Id;";

                Console.WriteLine("\n[SQL-Abfrage ausgeführt]\n");
                databaseConnection.Open();

                using (IDataReader reader = command.ExecuteReader())
                {
                    Console.WriteLine("╔════════════════════════════════════════════════════════════════════════════════╗");
                    Console.WriteLine("║                        FLÜGE & KUNDEN ÜBERSICHT                                 ║");
                    Console.WriteLine("║                  (Inkl. Flüge ohne Buchungen, Kunden ohne Flüge)                ║");
                    Console.WriteLine("╚════════════════════════════════════════════════════════════════════════════════╝");
                    
                    int rowCounter = 0;
                    while (reader.Read())
                    {
                        rowCounter++;
                        
                        // Werte auslesen (und auf NULL prüfen, da FULL JOIN leere Felder erzeugt)
                        string customerId = reader["CustomerId"] == DBNull.Value ? "[---]" : reader["CustomerId"].ToString().PadLeft(4);
                        string customerName = reader["FirstName"] == DBNull.Value || reader["LastName"] == DBNull.Value 
                            ? "(kein Kunde)" 
                            : $"{reader["FirstName"]} {reader["LastName"]}";
                        
                        string flightId = reader["FlightId"] == DBNull.Value ? "[---]" : reader["FlightId"].ToString().PadLeft(4);
                        string flightRoute = reader["Departure"] == DBNull.Value || reader["Destination"] == DBNull.Value 
                            ? "(kein Flug)" 
                            : $"{reader["Departure"]} → {reader["Destination"]}";
                        
                        string seats = reader["Seats"] == DBNull.Value ? "-" : reader["Seats"].ToString();

                        Console.WriteLine($"│ Kunde: {customerId} {customerName.PadRight(22)} │ Flug: {flightId} {flightRoute.PadRight(22)} │ Plätze: {seats.PadLeft(2)} │");
                    }
                    
                    Console.WriteLine("╚════════════════════════════════════════════════════════════════════════════════╝");
                    Console.WriteLine($"\n[Ergebnis] {rowCounter} Datensätze gefunden\n");
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
