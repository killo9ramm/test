using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RBClient
{
    static class TimeHelper
    {
        public static string ParceAmount(string amount, int direction)
        {
            if (amount == "")
                return "00:00";

            if (direction == 1)
            {
                return amount.Replace(".", ":");
            }
            if (direction == 0)
            {
                return amount.Replace(",", ":");
            }
            else 
            {
                return amount.Replace("/", ":");
            }
        }
    }
}