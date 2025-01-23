﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using S10269029D_PRG2Assignment;

namespace S10269029_PRG2Assignment
{
    public class DDJBFlight : Flight
    {
        public double RequestFee { get; set; }

        public DDJBFlight(string flightNumber, string origin, string destination, DateTime expectedTime, string status, double requestFee)
            : base(flightNumber, origin, destination, expectedTime, status)
        {
            RequestFee = requestFee;
        }

        public override double CalculateFees()
        {
            return 100.0 + RequestFee;
        }

        public override string ToString()
        {
            return "[DDJB Flight] " + base.ToString();
        }
    }
}
