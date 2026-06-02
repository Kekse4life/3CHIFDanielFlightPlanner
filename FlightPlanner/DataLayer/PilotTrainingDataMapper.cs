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
    class PilotTraining
    {
        public int PilotId { get; set; }
        public int TrainingId { get; set; }
        public DateTime Date { get; set; }
    }

    class PilotTrainingDataMapper
    {
        public String ConnectionString { get; set; }

        public PilotTrainingDataMapper(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        private PilotTraining ParseRecord(IDataReader pilotTrainingReader)
        {
            PilotTraining pilotTraining = new PilotTraining();
            pilotTraining.PilotId = pilotTrainingReader.GetInt32(0);
            pilotTraining.TrainingId = pilotTrainingReader.GetInt32(1);
            pilotTraining.Date = pilotTrainingReader.GetDateTime(2);
            return pilotTraining;
        }

        private List<PilotTraining> ReadPilotTrainings(string sqlCommandText)
        {
            List<PilotTraining> pilotTrainings = new List<PilotTraining>();

            using (DbConnection databaseConnection = new SqlConnection(this.ConnectionString))
            {
                IDbCommand pilotTrainingReadCommand = databaseConnection.CreateCommand();
                pilotTrainingReadCommand.CommandText = sqlCommandText;

                databaseConnection.Open();

                IDataReader pilotTrainingReader = pilotTrainingReadCommand.ExecuteReader();

                while (pilotTrainingReader.Read())
                {
                    PilotTraining pilotTraining = ParseRecord(pilotTrainingReader);
                    pilotTrainings.Add(pilotTraining);
                }

                return pilotTrainings;
            }
        }

        public List<PilotTraining> ReadPilotTrainings()
        {
            List<PilotTraining> pilotTrainings = ReadPilotTrainings("select * from PilotTraining;");
            return pilotTrainings;
        }

        public List<PilotTraining> ReadByPilotId(int pilotId)
        {
            String sqlCommandText = $"select * from PilotTraining where PilotTraining.PilotId = {pilotId};";
            List<PilotTraining> pilotTrainings = ReadPilotTrainings(sqlCommandText);
            return pilotTrainings;
        }

        public List<PilotTraining> ReadByTrainingId(int trainingId)
        {
            String sqlCommandText = $"select * from PilotTraining where PilotTraining.TrainingId = {trainingId};";
            List<PilotTraining> pilotTrainings = ReadPilotTrainings(sqlCommandText);
            return pilotTrainings;
        }

        public PilotTraining Read(int pilotId, int trainingId)
        {
            String sqlCommandText = $"select * from PilotTraining where PilotTraining.PilotId = {pilotId} and PilotTraining.TrainingId = {trainingId};";
            List<PilotTraining> pilotTrainings = ReadPilotTrainings(sqlCommandText);
            
            if (pilotTrainings.Count > 0)
            {
                return pilotTrainings[0];
            }
            return null;
        }

        public int Create(PilotTraining pilotTraining)
        {
            using (DbConnection databaseConnection = new SqlConnection(this.ConnectionString))
            {
                IDbCommand createPilotTrainingCommand = databaseConnection.CreateCommand();
                createPilotTrainingCommand.CommandText =
                   $"insert into PilotTraining (PilotId, TrainingId, Date) " +
                   $"values ({pilotTraining.PilotId}, {pilotTraining.TrainingId}, " +
                   $"'{pilotTraining.Date.ToString("s", System.Globalization.CultureInfo.InvariantCulture)}');";

                Console.WriteLine(createPilotTrainingCommand.CommandText);
                databaseConnection.Open();

                int rowCount = createPilotTrainingCommand.ExecuteNonQuery();
                return rowCount;
            }
        }

        public int Update(PilotTraining pilotTraining)
        {
            using (DbConnection databaseConnection = new SqlConnection(this.ConnectionString))
            {
                IDbCommand updatePilotTrainingCommand = databaseConnection.CreateCommand();
                updatePilotTrainingCommand.CommandText =
                   $"update PilotTraining set Date = " +
                   $"'{pilotTraining.Date.ToString("s", System.Globalization.CultureInfo.InvariantCulture)}' " +
                   $"where PilotTraining.PilotId = {pilotTraining.PilotId} and PilotTraining.TrainingId = {pilotTraining.TrainingId};";

                Console.WriteLine(updatePilotTrainingCommand.CommandText);
                databaseConnection.Open();

                int rowCount = updatePilotTrainingCommand.ExecuteNonQuery();
                return rowCount;
            }
        }

        public int Delete(int pilotId, int trainingId)
        {
            using (DbConnection databaseConnection = new SqlConnection(this.ConnectionString))
            {
                IDbCommand deletePilotTrainingCommand = databaseConnection.CreateCommand();
                deletePilotTrainingCommand.CommandText = $"delete from PilotTraining where PilotTraining.PilotId = {pilotId} and PilotTraining.TrainingId = {trainingId};";

                Console.WriteLine(deletePilotTrainingCommand.CommandText);
                databaseConnection.Open();

                int rowCount = deletePilotTrainingCommand.ExecuteNonQuery();
                return rowCount;
            }
        }

        public int Delete(PilotTraining pilotTraining)
        {
            return Delete(pilotTraining.PilotId, pilotTraining.TrainingId);
        }

        public int DeleteByPilotId(int pilotId)
        {
            using (DbConnection databaseConnection = new SqlConnection(this.ConnectionString))
            {
                IDbCommand deletePilotTrainingCommand = databaseConnection.CreateCommand();
                deletePilotTrainingCommand.CommandText = $"delete from PilotTraining where PilotTraining.PilotId = {pilotId};";

                Console.WriteLine(deletePilotTrainingCommand.CommandText);
                databaseConnection.Open();

                int rowCount = deletePilotTrainingCommand.ExecuteNonQuery();
                return rowCount;
            }
        }
    }
}
