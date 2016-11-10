using System;

using Multinet.Sample;
using Multinet.Genetic;
using Multinet.Net;
using Multinet.Net.Impl;
using Multinet.Utils;
using Multinet.Math;

namespace Multinet.App
{
    class Program
    {
		public static void XOR()
		{
            XORProblem problem = new XORProblem();
            problem.Run();   
		}

		public static void MatrixPattern() {
			MatrixPatternDetectionProblem mp = new MatrixPatternDetectionProblem ();
			mp.Training ();
			mp.Run ();
		}

		public static void run1(){
			int[] st = new int[2];
			st [0] = 0;
			st [1] = 0;

			Multinet.Math.PRNG.SetSeed (Environment.TickCount);
			int len = 200;
			for (int i = 0; i < len; i++) {
				double c = Multinet.Math.PRNG.NextDouble();
				System.Console.WriteLine (c);
				if (c > 0.5) {
					st [0]++; 
				} else {
					st [1]++;
				}
			}

			Console.WriteLine ("Head: {0}%, Tail: {1}%", (int)(100 * (st[0]/(float)len) ), (int)(100 * (st [1]/(float)len) ) );
		}

        static void Main(string[] args)
        {
			MatrixPattern ();
			//XOR();
			//run1();
            Console.ReadKey();
        }
    }
}
