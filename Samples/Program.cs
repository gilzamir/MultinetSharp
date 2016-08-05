using System;

using Multinet.Sample;
using Multinet.Genetic;

namespace Multinet.App
{
    class Program
    {       

		public static void TestGenetic()
		{
            //XORProblem problem = new XORProblem();
            //problem.Run();   

            Problem problem = new Problem(genetic, evaluator, stopCondiction, new DebugProblemEventHandler());



        }

        static void Main(string[] args)
        {
            //Test.Test.testHomeoestaticNet();
            //Test.Test.testChromossome();
            //Test.Test.testBitArrayUtils();
            //Test.Test.testBeer1995Neuron();

             //Test.Test.testMutation();
            //Test.Test.testCrossover();
			TestGenetic();
            Console.ReadKey();
        }
    }
}
