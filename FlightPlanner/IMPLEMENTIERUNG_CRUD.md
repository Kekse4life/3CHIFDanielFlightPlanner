# FlightPlanner CRUD Implementation - Dokumentation

## Übersicht der Implementierten Anforderungen

Dieses Projekt implementiert **CRUD-Operationen (Create, Read, Update, Delete)** für **zwei 1:n Beziehungen** sowie **zusätzliche Funktionalität für bessere Noten**.

---

## 1. IMPLEMENTIERTE DATAMAPPER (1:n Beziehungen)

### 1.1 **Airline & Pilot** (1:n Beziehung) - NEUE IMPLEMENTIERUNG
- **AirlineDataMapper.cs** - Vollständige CRUD-Operationen für Airlines
  - `Create(Airline)` - Neue Airline erstellen
  - `Read(int id)` - Einzelne Airline laden
  - `ReadAirlines()` - Alle Airlines laden
  - `Update(Airline)` - Airline-Daten aktualisieren
  - `Delete(int id)` - Airline löschen

- **PilotDataMapper.cs** - Vollständige CRUD-Operationen für Piloten
  - `Create(Pilot)` - Neuen Piloten erstellen
  - `Read(int id)` - Einzelnen Piloten laden
  - `ReadPilots()` - Alle Piloten laden
  - `Update(Pilot)` - Piloten-Daten aktualisieren
  - `Delete(int id)` - Piloten löschen
  - `DeleteByAirlineId(int)` - Alle Piloten einer Airline löschen (für Repository)

- **AirlineRepository.cs** - Geschäftslogik für Airline-Löschung (mit Abhängigkeiten)
  - `DeleteAirlineAndItsPilots(int)` - Löscht Airline UND alle zugehörigen Piloten

### 1.2 **Training & PilotTraining** (1:n + m:n Beziehung) - ZUSÄTZLICHE IMPLEMENTIERUNG
- **TrainingDataMapper.cs** - Vollständige CRUD-Operationen für Trainings
  - `Create(Training)` - Neues Training erstellen
  - `Read(int id)` - Einzelnes Training laden
  - `ReadTrainings()` - Alle Trainings laden
  - `Update(Training)` - Training-Daten aktualisieren
  - `Delete(int id)` - Training löschen

- **PilotTrainingDataMapper.cs** - Vollständige CRUD-Operationen für m:n Beziehung
  - `Create(PilotTraining)` - Neue Pilot-Training-Zuordnung erstellen
  - `Read(int pilotId, int trainingId)` - Einzelne Zuordnung laden
  - `ReadByPilotId(int)` - Alle Trainings eines Piloten laden
  - `ReadByTrainingId(int)` - Alle Piloten eines Trainings laden
  - `Update(PilotTraining)` - Datum der Zuordnung ändern
  - `Delete(int, int)` - Spezifische Zuordnung löschen
  - `DeleteByPilotId(int)` - Alle Trainings eines Piloten löschen

### 1.3 **Customer & Booking** (1:n Beziehung) - BESTEHEND, ERWEITERT
- **CustomerDataMapper.cs** - Erweiterte CRUD-Operationen
  - `Create()` - Neuen Kunden anlegen
  - `Read()` - Kunden auslesen
  - `UpdateLastName()` - Nachnamen ändern
  - `Delete()` - Kunden löschen

- **BookingDataMapper.cs** - Erweiterte CRUD-Operationen
  - `Create()` - Neue Buchung erstellen (via Stored Procedure)
  - `Read(int flightId, int customerId)` - Einzelne Buchung laden ? IMPLEMENTIERT
  - `ReadBookings()` - Alle Buchungen laden
  - `Update()` - Buchungs-Daten aktualisieren ? IMPLEMENTIERT
  - `Delete()` - Buchung löschen ? IMPLEMENTIERT

- **CustomerRepository.cs** - Geschäftslogik für Kunde-Löschung (mit Abhängigkeiten)
  - `DeleteCustomerAndHisBookings()` - Löscht Kunde UND alle seine Buchungen

### 1.4 **Flight & Booking** (1:n Beziehung) - BESTEHEND (in Angabe implementiert)
- **FlightDataMapper.cs** - Vollständige CRUD-Operationen
- **FlightRepository.cs** - Geschäftslogik für Flight-Löschung (mit Abhängigkeiten)

---

## 2. DATENBANK-CONSTRAINTS

Die Implementierung beachtet folgende **Primär- und Fremdschlüssel-Constraints**:

| Beziehung | Constraint | Aktion |
|-----------|-----------|--------|
| Flight ? Booking | FK_Booking_Flight | ON DELETE NO ACTION |
| Customer ? Booking | FK_Booking_Customer | ON DELETE CASCADE |
| Airline ? Pilot | FK_Pilot_Airline | ON DELETE SET DEFAULT |
| Pilot ? PilotTraining | FK_PilotTraining_Pilot | ON DELETE CASCADE |
| Training ? PilotTraining | FK_PilotTraining_Training | ON DELETE CASCADE |

---

## 3. TESTFÄLLE IN Program.cs

### Test 1: Airline & Pilot (1:n Beziehung)
```
- CREATE: Neue Airline und 2 Piloten anlegen
- READ: Airline und Piloten auslesen
- UPDATE: Pilot-Daten aktualisieren
- DELETE: Einzelnen Piloten löschen (keine Abhängigkeiten)
- DELETE: Airline mit ALLEN Piloten löschen (1:n Abhängigkeit!)
```

### Test 2: Training & PilotTraining (1:n + m:n Beziehung)
```
- CREATE: Neue Trainings und Pilot-Training-Zuordnungen
- READ: Trainings und deren Zuordnungen auslesen
- UPDATE: Training und Zuordnungs-Datum ändern
- DELETE: Einzelne Zuordnung löschen
- DELETE: Training löschen (cascades zu PilotTraining)
- DELETE: Alle Trainings für Pilot löschen
```

### Test 3: Customer & Booking (1:n Beziehung - BESTEHEND)
```
- CREATE: Neuen Kunden anlegen
- READ: Kunden auslesen
- UPDATE: Nachnamen ändern
- DELETE: Kunden mit seinen Buchungen löschen (1:n Abhängigkeit!)
```

### Test 4: Flight & Booking (1:n Beziehung - BESTEHEND)
```
- READ: Alle Flüge auslesen
- CREATE: Neuen Flug ohne Buchungen
- UPDATE: Flug-Daten aktualisieren
- DELETE: Flug ohne Abhängigkeiten löschen
- DELETE: Flug mit SEINEN BUCHUNGEN löschen (1:n Abhängigkeit!)
```

---

## 4. ARCHITEKTUR-PATTERN

### Drei-Schichten-Modell:

```
???????????????????????????????????????????
?   PRESENTATION LAYER (Program.cs)       ?
?   - Test-Szenarien                      ?
?   - Benutzer-Ausgaben                   ?
???????????????????????????????????????????
                    ?
???????????????????????????????????????????
?   BUSINESS LOGIC LAYER (Repository)     ?
?   - AirlineRepository                   ?
?   - CustomerRepository                  ?
?   - FlightRepository                    ?
?   Verantwortung: 1:n Abhängigkeiten     ?
???????????????????????????????????????????
                    ?
???????????????????????????????????????????
?   DATA LAYER (DataMapper)               ?
?   - AirlineDataMapper                   ?
?   - PilotDataMapper                     ?
?   - TrainingDataMapper                  ?
?   - PilotTrainingDataMapper             ?
?   - CustomerDataMapper                  ?
?   - BookingDataMapper                   ?
?   - FlightDataMapper                    ?
?   Verantwortung: CRUD-Operationen       ?
???????????????????????????????????????????
                    ?
???????????????????????????????????????????
?   DATABASE LAYER (SQL Server)           ?
?   - Tabellen mit Constraints            ?
?   - Stored Procedures                   ?
???????????????????????????????????????????
```

### Single Responsibility Principle:
- **DataMapper**: Nur CRUD-Operationen, KEINE Geschäftslogik
- **Repository**: Geschäftslogik für Abhängigkeiten zwischen Tabellen
- **Entity-Klassen**: Nur Daten-Container (Airline, Pilot, Training, etc.)

---

## 5. SQL INJECTION WARNUNG

?? **Wichtiger Hinweis**: Die Implementierung nutzt String-Interpolation für SQL-Queries.
Dies ist für Lehrzwecke akzeptabel, aber in Produktionscode sollte **Parameterized Queries** verwendet werden:

```csharp
// ? Unsicher (aktuell verwendet):
$"insert into Airline values ({airline.Id}, '{airline.RegisteredCompanyName}', ...)"

// ? Sicher (Best Practice):
cmd.Parameters.AddWithValue("@CompanyName", airline.RegisteredCompanyName);
```

---

## 6. ZUSAMMENFASSUNG DER ANFORDERUNGEN

| Anforderung | Status | Implementierung |
|-----------|--------|-----------------|
| CRUD für 2 Mapper (1:n) | ? | Airline & Pilot, Customer & Booking |
| CRUD für weitere 1:n | ? | Training & PilotTraining |
| Test: Datensatz ohne Abhängigkeiten | ? | Pilot/Training-Löschung getestet |
| Test: Datensatz mit Abhängigkeiten | ? | Airline/Customer-Löschung mit Cascade |
| Repository-Klasse für Delete-Operationen | ? | AirlineRepository, CustomerRepository |
| Beachtung von FK-Constraints | ? | ON DELETE CASCADE/NO ACTION beachtet |

---

## 7. AUSFÜHRUNG DER TESTS

1. Projekt starten: `F5` in Visual Studio
2. Die Datenbank wird automatisch neu erstellt
3. Alle CRUD-Tests werden ausgeführt
4. Ergebnisse werden in der Konsole angezeigt
5. `Enter` drücken zum Beenden

---

**Erstellungsdatum**: 2024  
**Projekttyp**: .NET Framework 4.7.2  
**Datenbanktyp**: SQL Server LocalDB
