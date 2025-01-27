using S10269029_PRG2Assignment;
using System;
using System.Collections.Generic;
using System.IO;

namespace S10269029_PRG2Assignment
{
    class Program
    {
        static void Main(string[] args)
        {
            // File paths
            string airlinesFilePath = "airlines.csv";
            string boardingGatesFilePath = "boardinggates.csv";
            string flightsFilePath = "flights.csv";

            // Data structures
            Dictionary<string, Airline> airlines = new Dictionary<string, Airline>();
            List<BoardingGate> boardingGates = new List<BoardingGate>();
            Dictionary<string, Flight> flights = new Dictionary<string, Flight>();

            try
            {
                // Check file existence before loading
                if (!File.Exists(airlinesFilePath) || !File.Exists(boardingGatesFilePath) || !File.Exists(flightsFilePath))
                {
                    Console.WriteLine("Error: One or more CSV files are missing. Ensure all required files are in the directory.");
                    return;
                }

                // Load airlines
                Console.WriteLine("Loading Airlines...");
                airlines = LoadAirlines(airlinesFilePath);
                Console.WriteLine($"{airlines.Count} Airlines Loaded!");

                // Load boarding gates
                Console.WriteLine("Loading Boarding Gates...");
                boardingGates = LoadBoardingGates(boardingGatesFilePath);
                Console.WriteLine($"{boardingGates.Count} Boarding Gates Loaded!");

                // Load flights
                Console.WriteLine("Loading Flights...");
                flights = LoadFlightsFromFile(flightsFilePath);
                Console.WriteLine($"{flights.Count} Flights Loaded!");

                // Assign flights to airlines and gates
                AssignFlightsToAirlinesAndGates(flights, airlines, boardingGates);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            // Menu loop
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=============================================");
                Console.WriteLine("Welcome to Changi Airport Terminal 5");
                Console.WriteLine("=============================================");
                Console.WriteLine("1. List All Flights");
                Console.WriteLine("2. List Boarding Gates");
                Console.WriteLine("3. Assign a Boarding Gate to a Flight");
                Console.WriteLine("4. Create Flight");
                Console.WriteLine("5. Display Airline Flights");
                Console.WriteLine("6. Modify Flight Details");
                Console.WriteLine("7. Display Flight Schedule");
                Console.WriteLine("0. Exit");
                Console.WriteLine();
                Console.Write("Please select your option: ");
                string option = Console.ReadLine();

                try
                {
                    switch (option)
                    {
                        case "1":
                            // Feature: List all flights
                            ListAllFlights(flights);
                            break;
                        case "2":
                            // Feature: List all boarding gates
                            ListAllBoardingGates(boardingGates);
                            break;
                        case "3":
                            Console.WriteLine("Feature coming soon.");
                            break;
                        case "4":
                            Console.WriteLine("Feature coming soon.");
                            break;
                        case "5":
                            // Feature: Display full flight details from an airline
                            DisplayFullFlightDetailsFromAirline(airlines, boardingGates);
                            break;
                        case "6":
                            // Feature: Modify flight details
                            ModifyFlightDetails(airlines);
                            break;
                        case "7":
                            Console.WriteLine("Feature coming soon.");
                            break;
                        case "0":
                            Console.WriteLine("Exiting... Goodbye!");
                            return;
                        default:
                            Console.WriteLine("Invalid option! Please try again.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }

                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
        }

        // Feature 1: Load airlines
        static Dictionary<string, Airline> LoadAirlines(string filePath)
        {
            var airlineDictionary = new Dictionary<string, Airline>();
            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                reader.ReadLine(); // Skip header line
                while ((line = reader.ReadLine()) != null)
                {
                    var parts = line.Split(',');
                    if (parts.Length >= 2)
                    {
                        var airline = new Airline(parts[0].Trim(), parts[1].Trim()); // Name, Code
                        airlineDictionary[airline.Code] = airline;
                    }
                }
            }
            return airlineDictionary;
        }

        // Feature 2: Load boarding gates
        static List<BoardingGate> LoadBoardingGates(string filePath)
        {
            var boardingGates = new List<BoardingGate>();
            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                reader.ReadLine(); // Skip header line
                while ((line = reader.ReadLine()) != null)
                {
                    var parts = line.Split(',');
                    if (parts.Length >= 4)
                    {
                        var gate = new BoardingGate(
                            parts[0].Trim(),                  // GateName
                            bool.Parse(parts[1].Trim()),     // SupportsDDJB
                            bool.Parse(parts[2].Trim()),     // SupportsCFFT
                            bool.Parse(parts[3].Trim())      // SupportsLWTT
                        );
                        boardingGates.Add(gate);
                    }
                }
            }
            return boardingGates;
        }

        // Feature 3: Load flights
        static Dictionary<string, Flight> LoadFlightsFromFile(string filePath)
        {
            var flightsDictionary = new Dictionary<string, Flight>();
            using (StreamReader reader = new StreamReader(filePath))
            {
                reader.ReadLine(); // Skip header line
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    var values = line.Split(',');
                    if (values.Length == 5)
                    {
                        string flightNumber = values[0].Trim();
                        string origin = values[1].Trim();
                        string destination = values[2].Trim();
                        DateTime expectedTime = DateTime.Parse(values[3].Trim());
                        string status = values[4].Trim();

                        var flight = new Flight(flightNumber, origin, destination, expectedTime, status);
                        flightsDictionary[flightNumber] = flight;
                    }
                }
            }
            return flightsDictionary;
        }

        // Assign flights to airlines and gates
        static void AssignFlightsToAirlinesAndGates(
            Dictionary<string, Flight> flights,
            Dictionary<string, Airline> airlines,
            List<BoardingGate> gates)
        {
            int gateIndex = 0; // Keep track of the current gate
            foreach (var flight in flights.Values)
            {
                string airlineCode = flight.FlightNumber.Substring(0, 2); // Extract airline code
                if (airlines.ContainsKey(airlineCode))
                {
                    airlines[airlineCode].AddFlight(flight); // Add flight to airline
                }

                if (gateIndex < gates.Count)
                {
                    gates[gateIndex].Flight = flight; // Assign flight to gate
                    gateIndex++;
                }
            }
        }

        // Feature 4: List all boarding gates
        static void ListAllBoardingGates(List<BoardingGate> boardingGates)
        {
            Console.WriteLine("\nBoarding Gates in Terminal 5:");
            Console.WriteLine("---------------------------------------------------------------");
            Console.WriteLine($"{"Gate Name",-15} {"DDJB",-20} {"CFFT",-20} {"LWTT",-20}");
            Console.WriteLine("---------------------------------------------------------------");

            foreach (var gate in boardingGates)
            {
                Console.WriteLine($"{gate.GateName,-15} {gate.SupportsDDJB,-20} {gate.SupportsCFFT,-20} {gate.SupportsLWTT,-20}");
            }

            Console.WriteLine("---------------------------------------------------------------");
        }

        // Feature: List all flights
        static void ListAllFlights(Dictionary<string, Flight> flights)
        {
            Console.WriteLine("\nAll Flights:");
            foreach (var flight in flights.Values)
            {
                Console.WriteLine(flight);
            }
        }

        // Feature 7: Display full flight details from an airline
        static void DisplayFullFlightDetailsFromAirline(Dictionary<string, Airline> airlines, List<BoardingGate> boardingGates)
        {
            // Step 1: List all Airlines
            Console.WriteLine("===============================================");
            Console.WriteLine("List of Airlines for Changi Airport Terminal 5");
            Console.WriteLine("===============================================");
            Console.WriteLine($"{"Airline Code",-15} {"Airline Name",-25}");
            Console.WriteLine("-----------------------------------------------");
            foreach (var airline in airlines.Values)
            {
                Console.WriteLine($"{airline.Code,-15} {airline.Name,-25}");
            }
            Console.WriteLine("===============================================");

            // Step 2: Prompt user to enter a 2-Letter Airline Code
            Console.Write("Enter Airline Code: ");
            string airlineCode = Console.ReadLine()?.ToUpper();

            // Step 3: Retrieve the selected Airline
            if (!airlines.TryGetValue(airlineCode, out var selectedAirline))
            {
                Console.WriteLine("Invalid Airline Code! Returning to the main menu.");
                return;
            }

            // Step 4: Display flights for the selected Airline
            Console.WriteLine("===============================================");
            Console.WriteLine($"List of Flights for {selectedAirline.Name}");
            Console.WriteLine("===============================================");
            Console.WriteLine($"{"Flight Number",-15} {"Airline Name",-25} {"Origin",-20} {"Destination",-20} {"Expected",-25}");
            Console.WriteLine("-----------------------------------------------------------------------------------------------");

            foreach (var flight in selectedAirline.Flights.Values)
            {
                // Ensure flight details are displayed with consistent formatting
                Console.WriteLine($"{flight.FlightNumber,-15} {selectedAirline.Name,-25} {flight.Origin,-20} {flight.Destination,-20} {flight.ExpectedTime:dd/M/yyyy h:mm:ss tt,-25}");
            }
            Console.WriteLine("===============================================");

            // Step 5: Prompt user to select a Flight Number
            Console.Write("Enter Flight Number: ");
            string flightNumber = Console.ReadLine();

            if (!selectedAirline.Flights.TryGetValue(flightNumber, out var selectedFlight))
            {
                Console.WriteLine("Invalid Flight Number! Returning to the main menu.");
                return;
            }

            // Step 6: Display full details of the selected flight
            Console.WriteLine("\n===============================================");
            Console.WriteLine("Full Flight Details");
            Console.WriteLine("===============================================");
            Console.WriteLine($"Flight Number: {selectedFlight.FlightNumber}");
            Console.WriteLine($"Airline Name: {selectedAirline.Name}");
            Console.WriteLine($"Origin: {selectedFlight.Origin}");
            Console.WriteLine($"Destination: {selectedFlight.Destination}");
            Console.WriteLine($"Expected Departure/Arrival Time: {selectedFlight.ExpectedTime:dd/M/yyyy h:mm:ss tt}");

            // Special Request Code(s)
            Console.Write("Special Request Code(s): ");
            var specialRequests = new List<string>();
            var gate = boardingGates.FirstOrDefault(g => g.Flight == selectedFlight);
            if (gate != null)
            {
                if (gate.SupportsCFFT) specialRequests.Add("CFFT");
                if (gate.SupportsDDJB) specialRequests.Add("DDJB");
                if (gate.SupportsLWTT) specialRequests.Add("LWTT");
            }
            Console.WriteLine(specialRequests.Count > 0 ? string.Join(", ", specialRequests) : "None");

            // Boarding Gate
            Console.WriteLine($"Boarding Gate: {(gate != null ? gate.GateName : "None")}");
            Console.WriteLine("===============================================");
        }
       
        // Feature 8: Modify Flight Details
        static void ModifyFlightDetails(Dictionary<string, Airline> airlines)
        {
            // Step 1: List all Airlines
            Console.WriteLine("===============================================");
            Console.WriteLine("List of Airlines for Changi Airport Terminal 5");
            Console.WriteLine("===============================================");
            Console.WriteLine($"{"Airline Code",-15} {"Airline Name",-25}");
            Console.WriteLine("-----------------------------------------------");
            foreach (var airline in airlines.Values)
            {
                Console.WriteLine($"{airline.Code,-15} {airline.Name,-25}");
            }
            Console.WriteLine("===============================================");

            // Step 2: Prompt user to enter a 2-Letter Airline Code
            Console.Write("Enter Airline Code: ");
            string airlineCode = Console.ReadLine()?.ToUpper();

            // Step 3: Retrieve the selected Airline
            if (!airlines.TryGetValue(airlineCode, out var selectedAirline))
            {
                Console.WriteLine("Invalid Airline Code! Returning to the main menu.");
                return;
            }

            // Step 4: Display flights for the selected Airline
            Console.WriteLine("===============================================");
            Console.WriteLine($"List of Flights for {selectedAirline.Name}");
            Console.WriteLine("===============================================");
            Console.WriteLine($"{"Flight Number",-15} {"Origin",-20} {"Destination",-20} {"Expected",-25}");
            Console.WriteLine("-----------------------------------------------------------------------------------------------");

            foreach (var flight in selectedAirline.Flights.Values)
            {
                Console.WriteLine($"{flight.FlightNumber,-15} {flight.Origin,-20} {flight.Destination,-20} {flight.ExpectedTime:dd/M/yyyy h:mm:ss tt,-25}");
            }
            Console.WriteLine("===============================================");

            // Step 5: Prompt the user to choose to modify or delete a flight
            Console.WriteLine("1. Modify Flight");
            Console.WriteLine("2. Delete Flight");
            Console.Write("Choose an option: ");
            string modifyOrDeleteOption = Console.ReadLine();

            if (modifyOrDeleteOption == "1")
            {
                // Modify Flight
                ModifyFlight(selectedAirline);
            }
            else if (modifyOrDeleteOption == "2")
            {
                // Delete Flight
                DeleteFlight(selectedAirline);
            }
            else
            {
                Console.WriteLine("Invalid option! Returning to the main menu.");
            }
        }

        // Helper Method: Modify a Flight
        static void ModifyFlight(Airline selectedAirline)
        {
            Console.Write("Enter Flight Number to modify: ");
            string flightNumber = Console.ReadLine();

            if (!selectedAirline.Flights.TryGetValue(flightNumber, out var selectedFlight))
            {
                Console.WriteLine("Invalid Flight Number! Returning to the main menu.");
                return;
            }

            Console.WriteLine("1. Modify Basic Information (Origin, Destination, Expected Time)");
            Console.WriteLine("2. Modify Status");
            Console.WriteLine("3. Modify Special Request Code");
            Console.Write("Choose an option: ");
            string modifyOption = Console.ReadLine();

            switch (modifyOption)
            {
                case "1":
                    Console.Write("Enter new Origin: ");
                    selectedFlight.Origin = Console.ReadLine();
                    Console.Write("Enter new Destination: ");
                    selectedFlight.Destination = Console.ReadLine();
                    Console.Write("Enter new Expected Departure/Arrival Time (dd/MM/yyyy HH:mm): ");
                    if (DateTime.TryParse(Console.ReadLine(), out var newExpectedTime))
                    {
                        selectedFlight.ExpectedTime = newExpectedTime;
                    }
                    else
                    {
                        Console.WriteLine("Invalid date format. Returning to the main menu.");
                    }
                    break;

                case "2":
                    Console.Write("Enter new Status: ");
                    selectedFlight.Status = Console.ReadLine();
                    break;

                case "3":
                    Console.Write("Enter new Special Request Code (e.g., CFFT): ");
                    string specialRequest = Console.ReadLine()?.ToUpper();
                    Console.WriteLine($"Special Request Code updated to: {specialRequest}");
                    break;

                default:
                    Console.WriteLine("Invalid option! Returning to the main menu.");
                    break;
            }

            Console.WriteLine("Flight updated!");
            DisplayFlightDetails(selectedFlight, selectedAirline.Name);
        }

        // Helper Method: Delete a Flight
        static void DeleteFlight(Airline selectedAirline)
        {
            Console.Write("Enter Flight Number to delete: ");
            string flightNumber = Console.ReadLine();

            if (!selectedAirline.Flights.TryGetValue(flightNumber, out var selectedFlight))
            {
                Console.WriteLine("Invalid Flight Number! Returning to the main menu.");
                return;
            }

            Console.Write($"Are you sure you want to delete Flight {flightNumber}? [Y/N]: ");
            string confirmation = Console.ReadLine()?.ToUpper();

            if (confirmation == "Y")
            {
                selectedAirline.Flights.Remove(flightNumber);
                Console.WriteLine($"Flight {flightNumber} has been deleted.");
            }
            else
            {
                Console.WriteLine("Deletion cancelled.");
            }
        }

        // Helper Method: Display Full Flight Details
        static void DisplayFlightDetails(Flight flight, string airlineName)
        {
            Console.WriteLine("\n===============================================");
            Console.WriteLine("Updated Flight Details");
            Console.WriteLine("===============================================");
            Console.WriteLine($"Flight Number: {flight.FlightNumber}");
            Console.WriteLine($"Airline Name: {airlineName}");
            Console.WriteLine($"Origin: {flight.Origin}");
            Console.WriteLine($"Destination: {flight.Destination}");
            Console.WriteLine($"Expected Departure/Arrival Time: {flight.ExpectedTime:dd/M/yyyy h:mm:ss tt}");
            Console.WriteLine($"Status: {flight.Status}");
            Console.WriteLine("===============================================");
        }
    } 
}
