using S10269029_PRG2Assignment;
using System;
using System.Collections.Generic;
using System.IO;


//==========================================================
// Student Number	: S10269029D
// Student Name	: Koh Rui Qi Rachael
// Partner Name	: Puteri Mayangsari Binte Abdul Haafiz
//==========================================================


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

                // Pause to allow visibility before clearing the screen
                Console.WriteLine("\nPress any key to continue to the menu...");
                Console.ReadKey();

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
                Console.Clear(); // Clear AFTER allowing user to see the previous output
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
                Console.WriteLine("8. Process Unassigned Flights");
                Console.WriteLine("9. Display the total fee per airline for the day");
                Console.WriteLine("0. Exit");
                Console.WriteLine();
                Console.Write("Please select your option: ");
                string option = Console.ReadLine();

                try
                {
                    switch (option)
                    {
                        case "1":
                            ListAllFlights(flights);
                            break;
                        case "2":
                            ListAllBoardingGates(boardingGates);
                            break;
                        case "3":
                            AssignBoardingGateToFlight(flights, boardingGates);
                            break;
                        case "4":
                            CreateNewFlight();
                            break;
                        case "5":
                            DisplayFullFlightDetailsFromAirline(airlines, boardingGates);
                            break;
                        case "6":
                            ModifyFlightDetails(airlines);
                            break;
                        case "7":
                            DisplayScheduledFlights(flights);
                            break;
                        case "8":
                            ProcessUnassignedFlights(flights, boardingGates);
                            break;
                        case "9":
                            DisplayTotalFeePerAirline(airlines, boardingGates);
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

            try
            {
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
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading airlines: {ex.Message}");
            }

            return airlineDictionary;
        }


        // Load BoardingGate
        static List<BoardingGate> LoadBoardingGates(string filePath)
        {
            var boardingGates = new List<BoardingGate>();

            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    reader.ReadLine(); // Skip header line
                    while ((line = reader.ReadLine()) != null)
                    {
                        var parts = line.Split(',');

                        if (parts.Length >= 4)
                        {
                            try
                            {
                                var gate = new BoardingGate(
                                    parts[0].Trim(), // Gate Name
                                    bool.TryParse(parts[1].Trim(), out bool supportsDDJB) ? supportsDDJB : false,
                                    bool.TryParse(parts[2].Trim(), out bool supportsCFFT) ? supportsCFFT : false,
                                    bool.TryParse(parts[3].Trim(), out bool supportsLWTT) ? supportsLWTT : false
                                );
                                boardingGates.Add(gate);
                            }
                            catch (Exception innerEx)
                            {
                                Console.WriteLine($"Skipping invalid data in boarding gates file: {innerEx.Message}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading boarding gates: {ex.Message}");
            }
            return boardingGates;
        }




        // Feature 2: Load flights
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

        // Feature 3: List all flights
        static void ListAllFlights(Dictionary<string, Flight> flights)
        {
            Console.WriteLine("\nAll Flights:");
            foreach (var flight in flights.Values)
            {
                Console.WriteLine(flight);
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

        // Feature 5: Assign a Boarding Gate to a Flight
        static void AssignBoardingGateToFlight(Dictionary<string, Flight> flights, List<BoardingGate> boardingGates)
        {
            // Step 1: Prompt user for the Flight Number
            Console.Write("Enter the Flight Number: ");
            string flightNumber = Console.ReadLine()?.ToUpper();

            // Step 2: Retrieve the selected Flight
            if (!flights.TryGetValue(flightNumber, out var selectedFlight))
            {
                Console.WriteLine("Invalid Flight Number! Returning to the main menu.");
                return;
            }

            // Step 3: Display basic Flight information
            Console.WriteLine("\nSelected Flight Details:");
            Console.WriteLine($"Flight Number: {selectedFlight.FlightNumber}");
            Console.WriteLine($"Origin: {selectedFlight.Origin}");
            Console.WriteLine($"Destination: {selectedFlight.Destination}");
            Console.WriteLine($"Expected Time: {selectedFlight.ExpectedTime:dd/MM/yyyy HH:mm}");
            Console.WriteLine($"Status: {selectedFlight.Status}");

            // Special Request Code(s)
            Console.Write("Special Request Code(s): ");
            var gate = boardingGates.FirstOrDefault(g => g.Flight == selectedFlight);
            if (gate != null)
            {
                List<string> specialRequests = new List<string>();
                if (gate.SupportsCFFT) specialRequests.Add("CFFT");
                if (gate.SupportsDDJB) specialRequests.Add("DDJB");
                if (gate.SupportsLWTT) specialRequests.Add("LWTT");
                Console.WriteLine(specialRequests.Count > 0 ? string.Join(", ", specialRequests) : "None");
            }
            else
            {
                Console.WriteLine("None");
            }

            // Step 4: Prompt user for the Boarding Gate
            Console.WriteLine("\nAvailable Boarding Gates:");
            foreach (var boardingGate in boardingGates)
            {
                Console.WriteLine(boardingGate.GateName);
            }
            Console.Write("Enter Boarding Gate Name: ");
            string gateName = Console.ReadLine()?.ToUpper();

            // Step 5: Validate if the Boarding Gate is already assigned
            var selectedGate = boardingGates.FirstOrDefault(g => g.GateName.ToUpper() == gateName);

            if (selectedGate == null)
            {
                Console.WriteLine("Invalid Boarding Gate! Returning to the main menu.");
                return;
            }

            if (selectedGate.Flight != null)
            {
                Console.WriteLine("This Boarding Gate is already assigned to another flight. Please choose a different gate.");
                return;
            }

            // Step 6: Assign the Boarding Gate to the selected Flight
            selectedGate.Flight = selectedFlight;

            // Step 7: Display assigned Boarding Gate details
            Console.WriteLine("\nBoarding Gate Assignment Successful!");
            Console.WriteLine($"Assigned Gate: {selectedGate.GateName}");
            Console.WriteLine($"Flight Number: {selectedFlight.FlightNumber}");
            Console.WriteLine($"Origin: {selectedFlight.Origin}");
            Console.WriteLine($"Destination: {selectedFlight.Destination}");
            Console.WriteLine($"Expected Time: {selectedFlight.ExpectedTime:dd/MM/yyyy HH:mm}");

            // Step 8: Prompt user to update Flight Status
            Console.Write("Would you like to update the Flight Status? (Y/N): ");
            string updateStatus = Console.ReadLine()?.ToUpper();

            if (updateStatus == "Y")
            {
                Console.WriteLine("Select the new Status for the Flight:");
                Console.WriteLine("1. Delayed");
                Console.WriteLine("2. Boarding");
                Console.WriteLine("3. On Time");
                string statusChoice = Console.ReadLine();

                switch (statusChoice)
                {
                    case "1":
                        selectedFlight.Status = "Delayed";
                        break;
                    case "2":
                        selectedFlight.Status = "Boarding";
                        break;
                    case "3":
                        selectedFlight.Status = "On Time";
                        break;
                    default:
                        Console.WriteLine("Invalid option! Keeping status as 'On Time'.");
                        selectedFlight.Status = "On Time";
                        break;
                }
            }
            else
            {
                // Set default status as 'On Time'
                selectedFlight.Status = "On Time";
            }

            // Step 9: Display the updated Flight details with assigned Boarding Gate
            Console.WriteLine("\nUpdated Flight Details:");
            Console.WriteLine($"Flight Number: {selectedFlight.FlightNumber}");
            Console.WriteLine($"Origin: {selectedFlight.Origin}");
            Console.WriteLine($"Destination: {selectedFlight.Destination}");
            Console.WriteLine($"Expected Time: {selectedFlight.ExpectedTime:dd/MM/yyyy HH:mm}");
            Console.WriteLine($"Status: {selectedFlight.Status}");
            Console.WriteLine($"Assigned Boarding Gate: {selectedGate.GateName}");

            Console.WriteLine("Boarding Gate assignment completed successfully!");
        }


        // Feature 6: Create a new flight
        static Dictionary<string, Flight> flights = new Dictionary<string, Flight>();
        const string filePath = "flights.csv"; // CSV file to store flights
        static void DisplayFlightDetails(Flight flight)
        {
            Console.WriteLine("\nFlight Details:");
            Console.WriteLine($"Flight Number: {flight.FlightNumber}");
            Console.WriteLine($"Origin: {flight.Origin}");
            Console.WriteLine($"Destination: {flight.Destination}");
            Console.WriteLine($"Expected Time: {flight.ExpectedTime:yyyy-MM-dd HH:mm}");
            Console.WriteLine($"Status: {flight.Status}");
        }

        public static void AppendFlightToCSV(Flight flight)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(filePath, true)) // true means append mode
                {
                    sw.WriteLine($"{flight.FlightNumber},{flight.Origin},{flight.Destination},{flight.ExpectedTime},{flight.Status}");
                }
                Console.WriteLine("Flight successfully added to CSV file.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing to file: {ex.Message}");
            }
        }

        public static void CreateNewFlight()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Create a New Flight");

                // Step 1: Prompt user for mandatory flight details
                Console.Write("Enter Flight Number: ");
                string flightNumber = Console.ReadLine()?.ToUpper();

                if (flights.ContainsKey(flightNumber))
                {
                    Console.WriteLine("Flight already exists! Please enter a different Flight Number.");
                    continue;
                }

                Console.Write("Enter Origin Airport: ");
                string origin = Console.ReadLine();

                Console.Write("Enter Destination Airport: ");
                string destination = Console.ReadLine();

                Console.Write("Enter Expected Departure/Arrival Time (yyyy-MM-dd HH:mm): ");
                if (!DateTime.TryParse(Console.ReadLine(), out DateTime expectedTime))
                {
                    Console.WriteLine("Invalid date format! Please enter a valid date.");
                    continue;
                }

                // Step 2: Flight status (default = "On Time")
                string status = "On Time";

                // Step 3: Create Flight object using your constructor
                Flight newFlight = new Flight(flightNumber, origin, destination, expectedTime, status);

                // Step 4: Add flight to dictionary
                flights.Add(flightNumber, newFlight);

                // Step 5: Append flight data to flights.csv
                AppendFlightToCSV(newFlight);

                Console.WriteLine("\nFlight added successfully!");
                DisplayFlightDetails(newFlight);

                // Step 6: Ask if user wants to add another flight
                Console.Write("\nWould you like to add another Flight? (Y/N): ");
                string addAnother = Console.ReadLine()?.ToUpper();
                if (addAnother != "Y")
                {
                    Console.WriteLine("Returning to main menu...");
                    break;
                }
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

        // Feature 9: Display scheduled flights in chronological order, with boarding gates assignments where applicable
        static void DisplayScheduledFlights(Dictionary<string, Flight> flights)
        {
            Console.Clear();
            Console.WriteLine("Scheduled Flights in Chronological Order\n");

            // Sort flights by ExpectedTime
            var sortedFlights = flights.Values.OrderBy(f => f.ExpectedTime).ToList();

            if (!sortedFlights.Any())
            {
                Console.WriteLine("No flights scheduled for the day.");
                return;
            }

            foreach (var flight in sortedFlights)
            {
                Console.WriteLine(flight); // Calls the overridden ToString method
                Console.WriteLine(new string('-', 50));
            }
        }

        // Advanced Features : Part (a)

        // Process all unassigned flights to boarding gates in bulk
        static void ProcessUnassignedFlights(Dictionary<string, Flight> flights, List<BoardingGate> boardingGates)
        {
            Queue<Flight> flightQueue = new Queue<Flight>();

            // Step 1: Identify Flights without a Boarding Gate assigned
            foreach (var flight in flights.Values)
            {
                if (string.IsNullOrEmpty(flight.BoardingGate)) // Flight has no assigned gate
                {
                    flightQueue.Enqueue(flight);
                }
            }

            Console.WriteLine("\n===============================================");
            Console.WriteLine($"Total Flights without a Boarding Gate: {flightQueue.Count}");

            // Step 2: Identify Boarding Gates that do not have a Flight assigned
            List<BoardingGate> unassignedGates = boardingGates.Where(g => g.Flight == null).ToList();
            Console.WriteLine($"Total Boarding Gates without an assigned Flight: {unassignedGates.Count}");
            Console.WriteLine("===============================================");

            int flightsAssigned = 0;
            int gatesAssigned = 0;

            // Step 3: Process each Flight in the queue
            while (flightQueue.Count > 0 && unassignedGates.Count > 0)
            {
                Flight currentFlight = flightQueue.Dequeue(); // Dequeue first flight

                // Step 4: Check if Flight has a Special Request Code
                BoardingGate assignedGate = null;
                bool hasSpecialRequest = !string.IsNullOrEmpty(currentFlight.SpecialRequestCode);

                if (hasSpecialRequest)
                {
                    // Step 5: Try to find a gate that matches the Special Request Code
                    assignedGate = unassignedGates.FirstOrDefault(g =>
                        (currentFlight.SpecialRequestCode == "CFFT" && g.SupportsCFFT) ||
                        (currentFlight.SpecialRequestCode == "DDJB" && g.SupportsDDJB) ||
                        (currentFlight.SpecialRequestCode == "LWTT" && g.SupportsLWTT));
                }

                // Step 6: If no matching gate is found or no special request exists, assign a general gate
                if (assignedGate == null)
                {
                    assignedGate = unassignedGates.FirstOrDefault(g => g.Flight == null);
                }

                // Step 7: Assign the Boarding Gate to the Flight Number
                if (assignedGate != null)
                {
                    assignedGate.Flight = currentFlight;
                    currentFlight.BoardingGate = assignedGate.GateName;
                    unassignedGates.Remove(assignedGate);
                    flightsAssigned++;
                    gatesAssigned++;

                    // Step 8: Display the Flight details with Basic Information
                    Console.WriteLine("\n-----------------------------------------------");
                    Console.WriteLine($"Assigned Flight {currentFlight.FlightNumber} to Gate {assignedGate.GateName}");
                    Console.WriteLine("Flight Details:");
                    Console.WriteLine($"Flight Number: {currentFlight.FlightNumber}");
                    Console.WriteLine($"Origin: {currentFlight.Origin}");
                    Console.WriteLine($"Destination: {currentFlight.Destination}");
                    Console.WriteLine($"Expected Time: {currentFlight.ExpectedTime:yyyy-MM-dd HH:mm}");
                    Console.WriteLine($"Status: {currentFlight.Status}");
                    Console.WriteLine($"Special Request Code: {(hasSpecialRequest ? currentFlight.SpecialRequestCode : "N/A")}");
                    Console.WriteLine($"Boarding Gate: {currentFlight.BoardingGate}");
                    Console.WriteLine("-----------------------------------------------");
                }
            }

            // Step 9: Display the final summary of assignments
            Console.WriteLine("\n===============================================");
            Console.WriteLine($"Total Flights Assigned: {flightsAssigned}");
            Console.WriteLine($"Total Boarding Gates Assigned: {gatesAssigned}");

            int totalProcessed = flightsAssigned + gatesAssigned;
            double autoProcessingPercentage = totalProcessed > 0
                ? (totalProcessed / (double)(flights.Count + boardingGates.Count)) * 100
                : 0;

            Console.WriteLine($"Total Flights and Gates Processed: {totalProcessed}");
            Console.WriteLine($"Percentage of Automatic Assignments: {autoProcessingPercentage:F2}%");
            Console.WriteLine("===============================================");
        }


        // Advanced Feature: Part (b)
        // Feature 10: Display the total fee per airline for the day
        static void DisplayTotalFeePerAirline(Dictionary<string, Airline> airlines, List<BoardingGate> boardingGates)
        {
            // Step 1: Check if all flights have boarding gates assigned
            bool allFlightsAssigned = true;
            foreach (var airline in airlines.Values)
            {
                foreach (var flight in airline.Flights.Values)
                {
                    if (string.IsNullOrEmpty(flight.BoardingGate)) // Flight has no assigned gate
                    {
                        allFlightsAssigned = false;
                        break;
                    }
                }
            }

            if (!allFlightsAssigned)
            {
                Console.WriteLine("Error: Some flights have not been assigned Boarding Gates. Please assign all boarding gates before running this feature.");
                return;
            }

            // Step 2: Display fees for each airline
            double totalFees = 0;
            double totalDiscounts = 0;

            foreach (var airline in airlines.Values)
            {
                Console.WriteLine($"\n===============================================");
                Console.WriteLine($"Fees for Airline: {airline.Name}");
                Console.WriteLine($"===============================================");
                double airlineFeeSubtotal = 0;
                double airlineDiscountSubtotal = 0;

                // Step 3: Loop through each flight in the airline
                foreach (var flight in airline.Flights.Values)
                {
                    double flightFee = 0;

                    // Apply Origin/Destination Fee (if origin or destination is Singapore)
                    double originDestinationFee = (flight.Origin == "SIN" || flight.Destination == "SIN") ? 800 : 500;
                    flightFee += originDestinationFee;

                    // Apply Special Request Fee (based on special request codes)
                    double specialRequestFee = 0;
                    if (!string.IsNullOrEmpty(flight.SpecialRequestCode))
                    {
                        switch (flight.SpecialRequestCode)
                        {
                            case "CFFT":
                                specialRequestFee = 100;  // Example fee for CFFT
                                break;
                            case "DDJB":
                                specialRequestFee = 150;  // Example fee for DDJB
                                break;
                            case "LWTT":
                                specialRequestFee = 200;  // Example fee for LWTT
                                break;
                            default:
                                specialRequestFee = 0;
                                break;
                        }
                    }
                    flightFee += specialRequestFee;

                    // Apply Boarding Gate Fee (base fee)
                    flightFee += 300;  // Boarding Gate Base Fee

                    // Compute Subtotal Fee for the flight
                    double flightDiscount = 0;
                    if (flight.Origin == "SIN" && flight.Destination == "SIN") // Example Promotional Condition (e.g., a discount for certain flights)
                    {
                        flightDiscount = 100;  // Example discount for flights departing and arriving at Singapore
                    }

                    // Calculate total fees and discounts
                    airlineFeeSubtotal += flightFee;
                    airlineDiscountSubtotal += flightDiscount;

                    // Display Flight Fee Breakdown
                    Console.WriteLine($"\nFlight Number: {flight.FlightNumber}");
                    Console.WriteLine($"   Origin: {flight.Origin}, Destination: {flight.Destination}");
                    Console.WriteLine($"   Special Request: {flight.SpecialRequestCode ?? "None"}");
                    Console.WriteLine($"   Boarding Gate Fee: $300");
                    Console.WriteLine($"   Origin/Destination Fee: ${originDestinationFee}");
                    Console.WriteLine($"   Special Request Fee: ${specialRequestFee}");
                    Console.WriteLine($"   Subtotal for Flight {flight.FlightNumber}: ${flightFee - flightDiscount}");
                    Console.WriteLine($"   Discount Applied: ${flightDiscount}");
                    Console.WriteLine("-----------------------------------------------");
                }

                // Step 4: Display Airline Fee Summary
                double totalAirlineFees = airlineFeeSubtotal - airlineDiscountSubtotal;
                Console.WriteLine($"\nTotal Fees for Airline {airline.Name}: ${airlineFeeSubtotal}");
                Console.WriteLine($"Total Discounts for Airline {airline.Name}: ${airlineDiscountSubtotal}");
                Console.WriteLine($"Final Total Fees for Airline {airline.Name}: ${totalAirlineFees}");
                totalFees += totalAirlineFees;
                totalDiscounts += airlineDiscountSubtotal;

                Console.WriteLine("===============================================");
            }

            // Step 5: Display Final Summary for all Airlines
            double grandTotalFees = totalFees - totalDiscounts;
            double discountPercentage = totalFees > 0 ? (totalDiscounts / totalFees) * 100 : 0;

            Console.WriteLine("\n===============================================");
            Console.WriteLine("Final Summary of Airline Fees for the Day");
            Console.WriteLine("===============================================");
            Console.WriteLine($"Total Fees from all Airlines: ${totalFees}");
            Console.WriteLine($"Total Discounts from all Airlines: ${totalDiscounts}");
            Console.WriteLine($"Final Total Fees (after discounts): ${grandTotalFees}");
            Console.WriteLine($"Total Discount Percentage: {discountPercentage:F2}%");
            Console.WriteLine("===============================================");
        }

    }

}


