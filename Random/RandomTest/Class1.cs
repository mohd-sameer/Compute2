using System;
using Random;

namespace RandomTest
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	class Class1
	{
		static void Main(string[] args)
		{
			//
			// TODO: Add code to start application here
			//
			cGaussianRandom R = new cGaussianRandom(0, -100, 20);
			for (int i = 1; i < 100; i++) 
			{
				Console.Write(R.Value);
				Console.Write(" ");
			}
			Console.ReadLine();
		}
	}
}
