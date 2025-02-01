using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//==========================================================
// Student Number	: S10269029D
// Student Name	: Koh Rui Qi Rachael
// Partner Name	: Puteri Mayangsari Binte Abdul Haafiz
//==========================================================

namespace S10269029_PRG2Assignment
{
    public class Flight : IComparable<Flight>
    {
        public string FlightNumber { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public DateTime ExpectedTime { get; set; }
        public string Status { get; set; }
        public string SpecialRequestCode { get; set; } // Optional
        public string BoardingGate { get; set; } // Optional

        public Flight(string flightNumber, string origin, string destination, DateTime expectedTime, string status, string specialRequestCode = null, string boardingGate = null)
        {
            FlightNumber = flightNumber;
            Origin = origin;
            Destination = destination;
            ExpectedTime = expectedTime;
            Status = status;
            SpecialRequestCode = specialRequestCode;
            BoardingGate = boardingGate;
        }

        public int CompareTo(Flight other)
        {
            return ExpectedTime.CompareTo(other.ExpectedTime);
        }

        public virtual double CalculateFees()
        {
            // Default fee calculation logic (example)
            double baseFee = 100.0;
            return baseFee;
        }

        public override string ToString()
        {
            return $"Flight Number: {FlightNumber}, Origin: {Origin}, Destination: {Destination}, " +
                   $"Expected Time: {ExpectedTime:yyyy-MM-dd HH:mm}, Status: {Status}, " +
                   $"Special Request Code: {SpecialRequestCode ?? "N/A"}, Boarding Gate: {BoardingGate ?? "N/A"}";
        }
    }
}












