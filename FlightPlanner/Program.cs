using FlightPlanner.BusinessLogicLayer;
using FlightPlanner.DataLayer;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightPlanner
{
    class Program
    {

        static void Main(string[] args)
        {
            // Programm verwendet ADO.NET API Connected Layer, Alternativen: ADO.NET Disconnected Layer, ADO.NET Entity Framework
            try
            {
                int rowCount = -2;

                string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDb;Initial Catalog=FlightPlanner;Integrated Security=SSPI";

                // Datenbank neu erstellen
                TestHelper.InitializeDatabase(connectionString);

                Console.WriteLine("========== CRUD TESTS FÜR MULTIPLE MAPPER-KLASSEN ==========\n");

                // ========== TEST 1: AIRLINE & PILOT (1:n Beziehung) ==========
                Console.WriteLine("========== TEST 1: AIRLINE & PILOT DATAMAPPER ==========\n");
                TestAirlineAndPilot(connectionString);

                // ========== TEST 2: TRAINING & PILOTTRAINING (1:n Beziehung) ==========
                Console.WriteLine("\n========== TEST 2: TRAINING & PILOTTRAINING DATAMAPPER ==========\n");
                TestTrainingAndPilotTraining(connectionString);

                // ========== TEST 3: CUSTOMER & BOOKING (1:n Beziehung) - BESTEHEND ==========
                Console.WriteLine("\n========== TEST 3: CUSTOMER & BOOKING DATAMAPPER (BESTEHEND) ==========\n");
                TestCustomerAndBooking(connectionString);

                // ========== TEST 4: FLIGHT & BOOKING (1:n Beziehung) - BESTEHEND ==========
                Console.WriteLine("\n========== TEST 4: FLIGHT & BOOKING DATAMAPPER (BESTEHEND) ==========\n");
                TestFlightAndBooking(connectionString);

                Console.WriteLine("\n========== ALLE TESTS ABGESCHLOSSEN ==========");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FEHLER: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
            }
            Console.WriteLine("\nPress enter to stop the program.");
            Console.ReadLine();
        }

        static void TestAirlineAndPilot(string connectionString)
        {
            Console.WriteLine("--- A. CREATE: Neue Airline und Piloten anlegen ---");
            AirlineDataMapper airlineMapper = new AirlineDataMapper(connectionString);
            PilotDataMapper pilotMapper = new PilotDataMapper(connectionString);

            Airline newAirline = new Airline
            {
                Id = 999,
                RegisteredCompanyName = "Test Airline",
                Country = "Switzerland",
                HeadQuarters = "Zurich"
            };
            rowCount = airlineMapper.Create(newAirline);
            Console.WriteLine($"[Result] Airline erstellt. Rows affected: {rowCount}\n");

            Pilot newPilot1 = new Pilot
            {
                Id = 9001,
                LastName = "Müller",
                Birthday = new DateTime(1990, 5, 15),
                Qualification = "Captain",
                FlightHours = 5000
            };
            rowCount = pilotMapper.Create(newPilot1);
            Console.WriteLine($"[Result] Pilot 1 erstellt. Rows affected: {rowCount}\n");

            Pilot newPilot2 = new Pilot
            {
                Id = 9002,
                LastName = "Schmidt",
                Birthday = new DateTime(1995, 8, 22),
                Qualification = "Copilot",
                FlightHours = 2000
            };
            rowCount = pilotMapper.Create(newPilot2);
            Console.WriteLine($"[Result] Pilot 2 erstellt. Rows affected: {rowCount}\n");

            Console.WriteLine("--- B. READ: Airline und ihre Piloten lesen ---");
            Airline readAirline = airlineMapper.Read(999);
            Console.WriteLine($"[DB-Ergebnis] Airline: {readAirline.RegisteredCompanyName} ({readAirline.Country})\n");

            Pilot readPilot = pilotMapper.Read(9001);
            Console.WriteLine($"[DB-Ergebnis] Pilot: {readPilot.LastName}, Qualification: {readPilot.Qualification}, FlightHours: {readPilot.FlightHours}\n");

            Console.WriteLine("--- C. UPDATE: Pilot-Daten aktualisieren ---");
            newPilot1.FlightHours = 5500;
            newPilot1.Qualification = "Copilot";
            rowCount = pilotMapper.Update(newPilot1);
            Console.WriteLine($"[Result] Pilot aktualisiert. Rows affected: {rowCount}\n");

            Console.WriteLine("--- D. DELETE: Einzelnen Pilot löschen (ohne Abhängigkeiten) ---");
            rowCount = pilotMapper.Delete(9002);
            Console.WriteLine($"[Result] Pilot 9002 gelöscht. Rows affected: {rowCount}\n");

            Console.WriteLine("--- E. DELETE: Airline mit ALLEN ihren Piloten löschen (1:n Abhängigkeit) ---");
            AirlineRepository airlineRepo = new AirlineRepository(connectionString);
            try
            {
                airlineRepo.DeleteAirlineAndItsPilots(999);
                Console.WriteLine("[Result] Airline und alle ihre Piloten wurden erfolgreich gelöscht.\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Fehler] {ex.Message}\n");
            }
        }

        static void TestTrainingAndPilotTraining(string connectionString)
        {
            Console.WriteLine("--- A. CREATE: Neue Trainings und PilotTraining-Zuordnungen anlegen ---");
            TrainingDataMapper trainingMapper = new TrainingDataMapper(connectionString);
            PilotTrainingDataMapper pilotTrainingMapper = new PilotTrainingDataMapper(connectionString);

            Training newTraining1 = new Training
            {
                Id = 9001,
                Description = "Advanced Flight Training",
                Level = 4
            };
            rowCount = trainingMapper.Create(newTraining1);
            Console.WriteLine($"[Result] Training 1 erstellt. Rows affected: {rowCount}\n");

            Training newTraining2 = new Training
            {
                Id = 9002,
                Description = "Safety Procedures",
                Level = 3
            };
            rowCount = trainingMapper.Create(newTraining2);
            Console.WriteLine($"[Result] Training 2 erstellt. Rows affected: {rowCount}\n");

            PilotTraining pilotTraining1 = new PilotTraining
            {
                PilotId = 111,  // Existierender Pilot
                TrainingId = 9001,
                Date = new DateTime(2024, 1, 15)
            };
            rowCount = pilotTrainingMapper.Create(pilotTraining1);
            Console.WriteLine($"[Result] PilotTraining 1 erstellt. Rows affected: {rowCount}\n");

            PilotTraining pilotTraining2 = new PilotTraining
            {
                PilotId = 111,
                TrainingId = 9002,
                Date = new DateTime(2024, 2, 20)
            };
            rowCount = pilotTrainingMapper.Create(pilotTraining2);
            Console.WriteLine($"[Result] PilotTraining 2 erstellt. Rows affected: {rowCount}\n");

            Console.WriteLine("--- B. READ: Training und zugehörige Pilot-Trainings lesen ---");
            Training readTraining = trainingMapper.Read(9001);
            Console.WriteLine($"[DB-Ergebnis] Training: {readTraining.Description}, Level: {readTraining.Level}\n");

            List<PilotTraining> pilotTrainings = pilotTrainingMapper.ReadByPilotId(111);
            Console.WriteLine($"[DB-Ergebnis] Pilot 111 hat {pilotTrainings.Count} Trainings:\n");
            foreach (PilotTraining pt in pilotTrainings)
            {
                Console.WriteLine($"  - TrainingId: {pt.TrainingId}, Date: {pt.Date:yyyy-MM-dd}");
            }
            Console.WriteLine();

            Console.WriteLine("--- C. UPDATE: Training aktualisieren ---");
            newTraining1.Description = "Advanced Flight Training - Updated";
            newTraining1.Level = 5;
            rowCount = trainingMapper.Update(newTraining1);
            Console.WriteLine($"[Result] Training aktualisiert. Rows affected: {rowCount}\n");

            Console.WriteLine("--- D. UPDATE: PilotTraining-Datum ändern ---");
            pilotTraining1.Date = new DateTime(2024, 3, 1);
            rowCount = pilotTrainingMapper.Update(pilotTraining1);
            Console.WriteLine($"[Result] PilotTraining aktualisiert. Rows affected: {rowCount}\n");

            Console.WriteLine("--- E. DELETE: Einzelne PilotTraining-Zuordnung löschen ---");
            rowCount = pilotTrainingMapper.Delete(111, 9002);
            Console.WriteLine($"[Result] PilotTraining (111, 9002) gelöscht. Rows affected: {rowCount}\n");

            Console.WriteLine("--- F. DELETE: Training löschen (cascades zu PilotTraining) ---");
            rowCount = trainingMapper.Delete(9001);
            Console.WriteLine($"[Result] Training 9001 gelöscht. Rows affected: {rowCount}\n");

            Console.WriteLine("--- G. DELETE: Alle Trainings für einen Pilot löschen ---");
            rowCount = pilotTrainingMapper.DeleteByPilotId(222);  // Existierender Pilot mit Trainings
            Console.WriteLine($"[Result] Alle Trainings für Pilot 222 gelöscht. Rows affected: {rowCount}\n");
        }

        static void TestCustomerAndBooking(string connectionString)
        {
            Console.WriteLine("--- A. CREATE: Neuen Kunden anlegen ---");
            CustomerDataMapper custMapper = new CustomerDataMapper(connectionString);

            custMapper.Create("Max", "Mustermann");
            Console.WriteLine("[Result] Kund(e) angelegt.\n");

            Console.WriteLine("--- B. READ: Kunden auslesen ---");
            custMapper.Read(1001);
            Console.WriteLine();

            Console.WriteLine("--- C. UPDATE: Nachnamen ändern ---");
            custMapper.UpdateLastName(1001, "Mustermann-Update");
            Console.WriteLine("[Result] Nachname aktualisiert.\n");

            Console.WriteLine("--- D. DELETE: Kunde mit seinen Buchungen löschen (1:n Abhängigkeit) ---");
            CustomerRepository repo = new CustomerRepository(connectionString);
            try
            {
                repo.DeleteCustomerAndHisBookings(1001);
                Console.WriteLine("[Result] Kunde und alle seine Buchungen wurden gelöscht.\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Fehler] {ex.Message}\n");
            }
        }

        static void TestFlightAndBooking(string connectionString)
        {
            Console.WriteLine("--- A. READ: Alle Flüge auslesen ---");
            FlightDataMapper flightMapper = new FlightDataMapper(connectionString);
            List<Flight> flights = flightMapper.ReadFlights();
            Console.WriteLine($"[DB-Ergebnis] {flights.Count} Flüge gefunden:\n");
            foreach (Flight f in flights)
            {
                Console.WriteLine($"  - {f.ToString()}");
            }
            Console.WriteLine();

            Console.WriteLine("--- B. CREATE: Neuen Flug anlegen (ohne Buchungen) ---");
            Flight testFlight = new Flight
            {
                Id = 1001,
                Departure = "Vienna",
                Destination = "Budapest",
                DepartureDate = new DateTime(2024, 6, 15),
                Duration = 40,
                PlaneId = 21
            };
            int rowCount = flightMapper.Create(testFlight);
            Console.WriteLine($"[Result] Neuer Flug erstellt. Rows affected: {rowCount}\n");

            Console.WriteLine("--- C. UPDATE: Flug-Informationen aktualisieren ---");
            testFlight.Duration = 50;
            rowCount = flightMapper.Update(testFlight);
            Console.WriteLine($"[Result] Flug aktualisiert. Rows affected: {rowCount}\n");

            Console.WriteLine("--- D. DELETE: Flug ohne Abhängigkeiten löschen ---");
            rowCount = flightMapper.Delete(testFlight);
            Console.WriteLine($"[Result] Flug 1001 gelöscht. Rows affected: {rowCount}\n");

            Console.WriteLine("--- E. DELETE: Flug mit ALLEN seinen Buchungen löschen (1:n Abhängigkeit) ---");
            FlightRepository flightRepo = new FlightRepository(connectionString);
            try
            {
                flightRepo.DeleteFlight(204);  // Flight mit Buchungen
                Console.WriteLine("[Result] Flug 204 und alle seine Buchungen wurden gelöscht.\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Fehler] {ex.Message}\n");
            }
        }

        static int rowCount = -2;
    }
}




// Single responsibility-Prinzip
// CRUD: Create, Read, Update, Delete Operationen sind in den Mapper-Klassen implementiert
// Repository-Klassen handhaben Abhängigkeiten zwischen Tabellen (1:n Beziehungen)