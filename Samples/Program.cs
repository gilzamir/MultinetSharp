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

        public static void MatrixPattern()
        {
            MatrixPatternOnlineProblem mp = new MatrixPatternOnlineProblem();
            mp.Training();
            mp.Run();
        }

        public static void run1()
        {
            int[] st = new int[2];
            st[0] = 0;
            st[1] = 0;

            Multinet.Math.PRNG.SetSeed(Environment.TickCount);
            int len = 200;
            for (int i = 0; i < len; i++)
            {
                double c = Multinet.Math.PRNG.NextDouble();
                System.Console.WriteLine(c);
                if (c > 0.5)
                {
                    st[0]++;
                }
                else
                {
                    st[1]++;
                }
            }

            Console.WriteLine("Head: {0}%, Tail: {1}%", (int)(100 * (st[0] / (float)len)), (int)(100 * (st[1] / (float)len)));
        }

        public static void testsimples()
        {
            Genome genome = new Genome();
            Chromossome chrGain = new Chromossome();
            chrGain.AddGene(0, 0.5);
            chrGain.AddGene(1, 0.5);
            chrGain.AddGene(2, 0.8);

            Chromossome chrSyn = new Chromossome();
            chrSyn.AddGene(0, 0.001);
            chrSyn.AddGene(1, 0.0);
            chrSyn.AddGene(2, 0.01);

            Chromossome chrBias = new Chromossome();
            chrBias.AddGene(0, 0.02);
            chrBias.AddGene(1, 1.0);
            chrBias.AddGene(2, 0.77);

            genome.AddChromossome(0, chrGain);
            genome.AddChromossome(1, chrSyn);
            genome.AddChromossome(2, chrBias);

            Multinet.Net.Ext.HIRON3 hiron = new Multinet.Net.Ext.HIRON3(1, 1, 1, genome);
            hiron.CreateParameters();
            hiron.CreateNeurons();
            hiron.CreateSynapses();

            NeuralNet net = hiron.Net;

            Console.WriteLine(net);

            double input = 0;
            ConsoleKey inputInfo = ConsoleKey.C;
            while (inputInfo == ConsoleKey.C)
            {
                Console.Write("Net Input: ");
                input = double.Parse(Console.ReadLine());
                net[0].ProccessInput(input);
                net.Proccess();
                System.Console.WriteLine("Net Output {0}", net[2].GetOutput());
                Console.Write("Pressione C para continuar ou outra tecla para sair...");
                inputInfo = Console.ReadKey().Key;
                Console.WriteLine();
            }
        }

        public static void Pattern()
        {

            MatrixPatternOfflineProblem problem = new MatrixPatternOfflineProblem();
            problem.Run();
        }

        static void Main(string[] args)
        {
            MatrixPattern();
            //XOR();
            //run1();
            Console.ReadKey();
        }
    }
}
