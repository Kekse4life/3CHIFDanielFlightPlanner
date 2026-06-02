using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace FlightPlanner.DataLayer
{
    class AirlineRepository
    {
        private PilotDataMapper pilotDataMapper;
        private AirlineDataMapper airlineDataMapper;
        string ConnectionString { get; set; }

        public AirlineRepository(string connectionString)
        {
            this.ConnectionString = connectionString;
            pilotDataMapper = new PilotDataMapper(this.ConnectionString);
            airlineDataMapper = new AirlineDataMapper(this.ConnectionString);
        }

        /// <summary>
        /// Delete an airline and all its pilots.
        /// This method handles the 1:n relationship between Airline and Pilot.
        /// </summary>
        /// <param name="airlineId">The ID of the airline to delete</param>
        public void DeleteAirlineAndItsPilots(int airlineId)
        {
            int rowCount = Int32.MinValue;
            try
            {
                // First delete all pilots of this airline
                rowCount = pilotDataMapper.DeleteByAirlineId(airlineId);
                Console.WriteLine($"[Result] {rowCount} Pilots deleted.");
                
                // Then delete the airline itself
                rowCount = airlineDataMapper.Delete(airlineId);
                Console.WriteLine($"[Result] {rowCount} Airline deleted.");
            }
            catch (DbException dbEx)
            {
                throw new InvalidOperationException("Airline could not be deleted!", dbEx);
            }
            catch (Exception)
            {
                throw;
            }

            if (rowCount < 1)
            {
                throw new InvalidOperationException("The specified airline could not be deleted.");
            }
        }
    }
}
