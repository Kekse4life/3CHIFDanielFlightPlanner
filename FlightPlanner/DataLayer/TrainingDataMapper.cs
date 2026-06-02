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
    class Training
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int Level { get; set; }
    }

    class TrainingDataMapper
    {
        public String ConnectionString { get; set; }

        public TrainingDataMapper(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        private Training ParseRecord(IDataReader trainingReader)
        {
            Training training = new Training();
            training.Id = trainingReader.GetInt32(0);
            training.Description = trainingReader.GetString(1);
            training.Level = trainingReader.GetInt32(2);
            return training;
        }

        private List<Training> ReadTrainings(string sqlCommandText)
        {
            List<Training> trainings = new List<Training>();

            using (DbConnection databaseConnection = new SqlConnection(this.ConnectionString))
            {
                IDbCommand trainingReadCommand = databaseConnection.CreateCommand();
                trainingReadCommand.CommandText = sqlCommandText;

                databaseConnection.Open();

                IDataReader trainingReader = trainingReadCommand.ExecuteReader();

                while (trainingReader.Read())
                {
                    Training training = ParseRecord(trainingReader);
                    trainings.Add(training);
                }

                return trainings;
            }
        }

        public List<Training> ReadTrainings()
        {
            List<Training> trainings = ReadTrainings("select * from Training;");
            return trainings;
        }

        public Training Read(int id)
        {
            String sqlCommandText = $"select * from Training where Training.Id = {id};";
            List<Training> trainings = ReadTrainings(sqlCommandText);
            
            if (trainings.Count > 0)
            {
                return trainings[0];
            }
            return null;
        }

        public int Create(Training training)
        {
            using (DbConnection databaseConnection = new SqlConnection(this.ConnectionString))
            {
                IDbCommand createTrainingCommand = databaseConnection.CreateCommand();
                createTrainingCommand.CommandText =
                   $"insert into Training (Id, Description, Level) " +
                   $"values ({training.Id}, '{training.Description}', {training.Level});";

                Console.WriteLine(createTrainingCommand.CommandText);
                databaseConnection.Open();

                int rowCount = createTrainingCommand.ExecuteNonQuery();
                return rowCount;
            }
        }

        public int Update(Training training)
        {
            using (DbConnection databaseConnection = new SqlConnection(this.ConnectionString))
            {
                IDbCommand updateTrainingCommand = databaseConnection.CreateCommand();
                updateTrainingCommand.CommandText =
                   $"update Training set Description = '{training.Description}', " +
                   $"Level = {training.Level} " +
                   $"where Training.Id = {training.Id};";

                Console.WriteLine(updateTrainingCommand.CommandText);
                databaseConnection.Open();

                int rowCount = updateTrainingCommand.ExecuteNonQuery();
                return rowCount;
            }
        }

        public int Delete(int id)
        {
            using (DbConnection databaseConnection = new SqlConnection(this.ConnectionString))
            {
                IDbCommand deleteTrainingCommand = databaseConnection.CreateCommand();
                deleteTrainingCommand.CommandText = $"delete from Training where Training.Id = {id};";

                Console.WriteLine(deleteTrainingCommand.CommandText);
                databaseConnection.Open();

                int rowCount = deleteTrainingCommand.ExecuteNonQuery();
                return rowCount;
            }
        }

        public int Delete(Training training)
        {
            return Delete(training.Id);
        }
    }
}
