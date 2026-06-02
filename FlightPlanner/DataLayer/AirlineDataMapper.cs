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
    class Airline
    {
        public int Id { get; set; }
        public string RegisteredCompanyName { get; set; }
        public string Country { get; set; }
        public string HeadQuarters { get; set; }
    }

    class AirlineDataMapper
    {
        public String ConnectionString { get; set; }

        public AirlineDataMapper(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        private Airline ParseRecord(IDataReader airlineReader)
        {
            Airline airline = new Airline();
            airline.Id = airlineReader.GetInt32(0);
            airline.RegisteredCompanyName = airlineReader.GetString(1);
            airline.Country = airlineReader.GetString(2);
            airline.HeadQuarters = airlineReader.GetString(3);
            return airline;
        }

        private List<Airline> ReadAirlines(string sqlCommandText)
        {
            List<Airline> airlines = new List<Airline>();

            using (DbConnection databaseConnection = new SqlConnection(this.ConnectionString))
            {
                IDbCommand airlineReadCommand = databaseConnection.CreateCommand();
                airlineReadCommand.CommandText = sqlCommandText;

                databaseConnection.Open();

                IDataReader airlineReader = airlineReadCommand.ExecuteReader();

                while (airlineReader.Read())
                {
                    Airline airline = ParseRecord(airlineReader);
                    airlines.Add(airline);
                }

                return airlines;
            }
        }

        public List<Airline> ReadAirlines()
        {
            List<Airline> airlines = ReadAirlines("select * from Airline;");
            return airlines;
        }

        public Airline Read(int id)
        {
            String sqlCommandText = $"select * from Airline where Airline.Id = {id};";
            List<Airline> airlines = ReadAirlines(sqlCommandText);
            
            if (airlines.Count > 0)
            {
                return airlines[0];
            }
            return null;
        }

        public int Create(Airline airline)
        {
            using (DbConnection databaseConnection = new SqlConnection(this.ConnectionString))
            {
                IDbCommand createAirlineCommand = databaseConnection.CreateCommand();
                createAirlineCommand.CommandText =
                   $"insert into Airline (Id, RegisteredCompanyName, Country, HeadQuarters) " +
                   $"values ({airline.Id}, '{airline.RegisteredCompanyName}', '{airline.Country}', '{airline.HeadQuarters}');";

                Console.WriteLine(createAirlineCommand.CommandText);
                databaseConnection.Open();

                int rowCount = createAirlineCommand.ExecuteNonQuery();
                return rowCount;
            }
        }

        public int Update(Airline airline)
        {
            using (DbConnection databaseConnection = new SqlConnection(this.ConnectionString))
            {
                IDbCommand updateAirlineCommand = databaseConnection.CreateCommand();
                updateAirlineCommand.CommandText =
                   $"update Airline set RegisteredCompanyName = '{airline.RegisteredCompanyName}', " +
                   $"Country = '{airline.Country}', " +
                   $"HeadQuarters = '{airline.HeadQuarters}' " +
                   $"where Airline.Id = {airline.Id};";

                Console.WriteLine(updateAirlineCommand.CommandText);
                databaseConnection.Open();

                int rowCount = updateAirlineCommand.ExecuteNonQuery();
                return rowCount;
            }
        }

        public int Delete(int id)
        {
            using (DbConnection databaseConnection = new SqlConnection(this.ConnectionString))
            {
                IDbCommand deleteAirlineCommand = databaseConnection.CreateCommand();
                deleteAirlineCommand.CommandText = $"delete from Airline where Airline.Id = {id};";

                Console.WriteLine(deleteAirlineCommand.CommandText);
                databaseConnection.Open();

                int rowCount = deleteAirlineCommand.ExecuteNonQuery();
                return rowCount;
            }
        }

        public int Delete(Airline airline)
        {
            return Delete(airline.Id);
        }
    }
}
