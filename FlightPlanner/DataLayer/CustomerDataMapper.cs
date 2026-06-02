using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightPlanner
{
    public class CustomerDataMapper
    {
        public string ConnectionString { get; set; }

        public CustomerDataMapper(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        public void Create(string firstName, string lastName)
        {
            using (DbConnection databaseConnection = new SqlConnection(this.ConnectionString))
            {
                IDbCommand command = databaseConnection.CreateCommand();
                command.CommandText = $"insert into Customer (FirstName, LastName) values ('{firstName}', '{lastName}');";

                Console.WriteLine(command.CommandText); // Konsolen-Ausgabe wie in deiner Vorlage
                databaseConnection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void Read(int id)
        {
            using (DbConnection databaseConnection = new SqlConnection(this.ConnectionString))
            {
                IDbCommand command = databaseConnection.CreateCommand();
                command.CommandText = $"select * from Customer where Customer.Id = {id};";

                Console.WriteLine(command.CommandText);
                databaseConnection.Open();

                using (IDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Console.WriteLine($"[DB-Ergebnis] Kunde ID {id}: {reader["FirstName"]} {reader["LastName"]}");
                    }
                    else
                    {
                        Console.WriteLine($"[DB-Ergebnis] Kein Kunde mit ID {id} gefunden.");
                    }
                }
            }
        }

        public int UpdateLastName(int id, string newName)
        {
            using (DbConnection databaseConnection = new SqlConnection(this.ConnectionString))
            {
                IDbCommand updateCustomerCommand = databaseConnection.CreateCommand();
                updateCustomerCommand.CommandText =
                    $"update Customer set LastName = '{newName}' where Customer.Id = {id};";

                Console.WriteLine(updateCustomerCommand.CommandText);
                databaseConnection.Open();

                int rowCount = updateCustomerCommand.ExecuteNonQuery();
                return rowCount;
            }
        }

        public int Delete(int id)
        {
            using (DbConnection databaseConnection = new SqlConnection(this.ConnectionString))
            {
                IDbCommand command = databaseConnection.CreateCommand();
                command.CommandText = $"delete from Customer where Customer.Id = {id};";

                Console.WriteLine(command.CommandText);
                databaseConnection.Open();

                int rowCount = command.ExecuteNonQuery();
                return rowCount;
            }
        }
    }
}
