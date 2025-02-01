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
    public class NORMFlight : Flight
    {
        public NORMFlight(string flightNumber, string origin, string destination, DateTime expectedTime, string status)
            : base(flightNumber, origin, destination, expectedTime, status) { }

        public override double CalculateFees()
        {
            return 100.0; // Base fee for normal flights
        }

        public override string ToString()
        {
            return "[Normal Flight] " + base.ToString();
        }
    }
}
