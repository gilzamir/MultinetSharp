using System;

using Multinet.Sample;
using Multinet.Genetic;
using Multinet.Net;
using Multinet.Net.Impl;
using Multinet.Utils;

namespace Multinet.App
{
    class Program
    {
		public static void XOR()
		{
            XORProblem problem = new XORProblem();
            problem.Run();   
		}

        static void Main(string[] args)
        {
			XOR();
            Console.ReadKey();
        }
    }
}
