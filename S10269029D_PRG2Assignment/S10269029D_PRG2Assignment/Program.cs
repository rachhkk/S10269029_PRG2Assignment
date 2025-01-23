using S10269029_PRG2Assignment;
using System.Globalization;

class Program
{
    static void Main()
    {
        string filePath = "flights.csv";  // Replace with the actual file path
        Dictionary<string, Flight> flightsDictionary = new Dictionary<string, Flight>();

        try
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                // Read the header line and ignore it
                reader.ReadLine();

                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] values = line.Split(',');  // Assuming CSV is comma-separated

                    if (values.Length == 5)
                    {
                        string flightNumber = values[0].Trim();
                        string origin = values[1].Trim();
                        string destination = values[2].Trim();
                        DateTime expectedTime = DateTime.Parse(values[3].Trim());
                        string status = values[4].Trim();


                        // Create Flight object and add to dictionary
                        Flight flight = new Flight(flightNumber, origin, destination, expectedTime, status);
                        flightsDictionary[flightNumber] = flight;
                    }
                }
            }

            // Output the loaded flight data
            foreach (var kvp in flightsDictionary)
            {
                Console.WriteLine(kvp.Value);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error reading file: " + ex.Message);
        }
    }
}