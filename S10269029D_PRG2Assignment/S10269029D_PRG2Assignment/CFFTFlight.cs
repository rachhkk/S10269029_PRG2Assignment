﻿using System;
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
    public class CFFTFlight : Flight
    {
        public double RequestFee { get; set; }

        public CFFTFlight(string flightNumber, string origin, string destination, DateTime expectedTime, string status, double requestFee)
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
            return "[CFFT Flight] " + base.ToString();
        }
    }
}
