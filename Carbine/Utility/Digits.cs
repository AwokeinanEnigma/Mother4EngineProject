using System;

namespace Carbine.Utility
{
	public class Digits
	{
		public static int Get(int number, int place)
		{
			return Math.Abs(number) / (int)Math.Pow(10.0, (double)(place - 1)) % 10;
		}

		public static int CountDigits(int number)
		{
			int result = 1;
			if (number != 0)
			{
				result = (int)(Math.Log10((double)Math.Abs(number)) + 1.0);
			}
			return result;
		}
	}
}
