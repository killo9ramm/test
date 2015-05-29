using System;

namespace RBServer
{
	internal class CUtilHelper
	{
		public CUtilHelper()
		{
		}

		public static string ParceAmount(string amount, int direction)
		{
			if (amount == "")
			{
				return "0";
			}
			if (direction == 2)
			{
				return amount.Replace("", "-");
			}
			if (direction == 1)
			{
				return amount.Replace(".", ",");
			}
			return amount.Replace(",", ".");
		}
	}
}