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
    class PilotDataMapper
    {
        public String ConnectionString { get; set; }

        public PilotDataMapper(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        private Pilot ParseRecord(IDataReader pilotReader)
        {
            Pilot pilot = new Pilot();
            pilot.Id = pilotReader.GetInt32(0);
            pilot.LastName = pilotReader.GetString(1);
            pilot.Birthday = pilotReader.GetDateTime(2);
            pilot.Qualification = pilotReader.GetString(3);
            pilot.FlightHours = pilotReader.GetInt32(4);
            return pilot;
        }

        private List<Pilot> ReadPilots(string sqlCommandText)
        {
            List<Pilot> pilots = new List<Pilot>();

            using (DbConnection databaseConnection = new SqlConnection(this.ConnectionString))
            {
                IDbCommand pilotReadCommand = databaseConnection.CreateCommand();
                pilotReadCommand.CommandText = sqlCommandText;

                databaseConnection.Open();

                IDataReader pilotReader = pilotReadCommand.ExecuteReader();

                while (pilotReader.Read())
                {
                    Pilot pilot = ParseRecord(pilotReader);
                    pilots.Add(pilot);
                }

                return pilots;
            }
        }

        public List<Pilot> ReadPilots()
        {
            List<Pilot> pilots = ReadPilots("select Id, LastName, Birthday, Qualification, FlightHours from Pilot;");
            return pilots;
        }

        public Pilot Read(int id)
        {
            String sqlCommandText = $"select Id, LastName, Birthday, Qualification, FlightHours from Pilot where Pilot.Id = {id};";
            List<Pilot> pilots = ReadPilots(sqlCommandText);
            
            if (pilots.Count > 0)
            {
                return pilots[0];
            }
            return null;
        }

        public int Create(Pilot pilot)
        {
            using (DbConnection databaseConnection = new SqlConnection(this.ConnectionString))
            {
                IDbCommand createPilotCommand = databaseConnection.CreateCommand();
                createPilotCommand.CommandText =
                   $"insert into Pilot (Id, LastName, Birthday, Qualification, FlightHours, FirstDate, AirlineId) " +
                   $"values ({pilot.Id}, '{pilot.LastName}', " +
                   $"'{pilot.Birthday.ToString("s", System.Globalization.CultureInfo.InvariantCulture)}', " +
                   $"'{pilot.Qualification}', {pilot.FlightHours}, '{DateTime.Now.ToString("s", System.Globalization.CultureInfo.InvariantCulture)}', -1);";

                Console.WriteLine(createPilotCommand.CommandText);
                databaseConnection.Open();

                int rowCount = createPilotCommand.ExecuteNonQuery();
                return rowCount;
            }
        }

        public int Update(Pilot pilot)
        {
            using (DbConnection databaseConnection = new SqlConnection(this.ConnectionString))
            {
                IDbCommand updatePilotCommand = databaseConnection.CreateCommand();
                updatePilotCommand.CommandText =
                   $"update Pilot set LastName = '{pilot.LastName}', " +
                   $"Birthday = '{pilot.Birthday.ToString("s", System.Globalization.CultureInfo.InvariantCulture)}', " +
                   $"Qualification = '{pilot.Qualification}', " +
                   $"FlightHours = {pilot.FlightHours} " +
                   $"where Pilot.Id = {pilot.Id};";

                Console.WriteLine(updatePilotCommand.CommandText);
                databaseConnection.Open();

                int rowCount = updatePilotCommand.ExecuteNonQuery();
                return rowCount;
            }
        }

        public int Delete(int id)
        {
            using (DbConnection databaseConnection = new SqlConnection(this.ConnectionString))
            {
                IDbCommand deletePilotCommand = databaseConnection.CreateCommand();
                deletePilotCommand.CommandText = $"delete from Pilot where Pilot.Id = {id};";

                Console.WriteLine(deletePilotCommand.CommandText);
                databaseConnection.Open();

                int rowCount = deletePilotCommand.ExecuteNonQuery();
                return rowCount;
            }
        }

        public int Delete(Pilot pilot)
        {
            return Delete(pilot.Id);
        }

        public int DeleteByAirlineId(int airlineId)
        {
            using (DbConnection databaseConnection = new SqlConnection(this.ConnectionString))
            {
                IDbCommand deletePilotCommand = databaseConnection.CreateCommand();
                deletePilotCommand.CommandText = $"delete from Pilot where Pilot.AirlineId = {airlineId};";

                Console.WriteLine(deletePilotCommand.CommandText);
                databaseConnection.Open();

                int rowCount = deletePilotCommand.ExecuteNonQuery();
                return rowCount;
            }
        }
    }
}
