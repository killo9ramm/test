using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RBClient
{
    static class CUtilHelper
    {
        /// <summary>
        /// Замена точки на запятую в строке
        /// </summary>
        /// <param name="amount">Сумма в формате строки для замены</param>
        /// <param name="direction">1 - если меняем . на , и 2, если меняем , на .</param>
        /// <returns></returns>
        public static string ParceAmount(string amount, int direction)
        {
            if (amount == "")
                return "0";

            if (direction == 1)
                return amount.Replace(".", ",");
            else
                return amount.Replace(",", ".");
        }
    }
}
