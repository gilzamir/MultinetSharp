using System;
using System.Collections.Generic;
using System.Linq;
using Multinet.Net;
using Multinet.Genetic;
using Multinet.Utils;
using Multinet.Net.Impl;

namespace Multinet.Sample
{


	using rnd = Multinet.Math.PRNG;

    public class XOREvaluator : Evaluator
    {
        public bool logEnabled = false;

        public bool LogEnabled
        {
            get
            {
                return this.logEnabled;
            }

            set
            {
                logEnabled = value;
            }
        }

        public double evaluate(Evaluable obj)
        {

            double[][] inputs = new double[4][];
            inputs[0] = new double[] { 0, 0 };
            inputs[1] = new double[] { 1, 0 };
            inputs[2] = new double[] { 0, 1 };
            inputs[3] = new double[] { 1, 1 };

            double[] outputs = { 0, 1, 1, 0 };

            double error = 0;

            NeuralNet net = (NeuralNet)obj;

            for (uint i = 0; i < inputs.Length; i++)
            {
                double[] input = inputs[i];
                net[0].ProccessInput(input[0]);
                net[1].ProccessInput(input[1]);
                net.Proccess();

                double y = net[2].GetOutput();

				double e = System.Math.Abs(y-outputs[i]);
                error += e * e;

                if (LogEnabled)
                {
                    Console.WriteLine("{0}, {1} : {2}", input[0], input[1], y);
                }
            }
            return  10.0 - error;
        }
    }



    class XORProblem
    {
        GeneticA genetic;
        XOREvaluator evaluator = new XOREvaluator();

		private static bool stopCondiction(Problem problem){
			//((XORProblem)problem)


			if (problem.Epoch > 1000) {
				return true;
			}

			return false;
		}

        public XORProblem()
        {
            genetic = new GeneticA(200);
            genetic.Elitism = 2;
            genetic.SurvivalRate = 0.5;
            genetic.MutationRate = 0.0005;
            genetic.MinPopulationSize = 200;


            genetic.GenomeBuilder = () =>
            {
                Genome gen = new Genome();
                Chromossome cr = new Chromossome();
                cr.AddGene(0, rnd.NextDouble());
                cr.AddGene(1, rnd.NextDouble());
                cr.AddGene(2, rnd.NextDouble());
                cr.AddGene(3, rnd.NextDouble());
                cr.AddGene(4, rnd.NextDouble());
                cr.AddGene(5, rnd.NextDouble());
                cr.AddGene(6, rnd.NextDouble());
                cr.AddGene(7, rnd.NextDouble());
                
                gen.AddChromossome(0, cr);
                return gen;
            };

            genetic.Translator = (Genome gen) =>
            {
                NeuralNet net = new NeuralNet();

				net.NumericalMethod = new Math.Impl.EulerMethod();
                int n1 = net.CreateNeuron();
                int n2 = net.CreateNeuron();
                int n3 = net.CreateNeuron();
				int n4 = net.CreateNeuron();

                Neuron ne1 = net[n1];
                Neuron ne2 = net[n2];
                Neuron ne3 = net[n3];
				Neuron ne4 = net[n4];

                ne1.Implementation = new Beer1995Neuron();
                ne2.Implementation = new Beer1995Neuron();
                ne3.Implementation = new Beer1995Neuron();
				ne4.Implementation = new Beer1995Neuron();

                
                Chromossome cr = gen.GetChromossome(0);

                net.CreateSynapse(n1, n3, 60 * BitArrayUtils.ToNDouble(cr.GetGene(0))-30);
                net.CreateSynapse(n2, n3, 60 * BitArrayUtils.ToNDouble(cr.GetGene(1))-30);
				net.CreateSynapse(n3, n4, 60 * BitArrayUtils.ToNDouble(cr.GetGene(2))-30);

                ne1.Implementation.UseNumericalMethod = false;
				ne1.Implementation["inputgain"] = 2 * BitArrayUtils.ToNDouble(cr.GetGene(3));
                ne1.Implementation["outputgain"] = 1.0;
                ne1.Implementation["inputweight"] = 0.0;
                ne1.Implementation["sensorweight"] = 1.0;
                ne1.TimeConst = 1.0;
                ne1.Implementation["bias"] = 0.0;

                ne2.Implementation.UseNumericalMethod = false;
				ne2.Implementation["inputgain"] = 2 * BitArrayUtils.ToNDouble(cr.GetGene(4));
                ne2.Implementation["outputgain"] = 1.0;
                ne2.Implementation["inputweight"] = 0.0;
                ne2.Implementation["sensorweight"] = 1.0;
                ne2.TimeConst = 1.0;
                ne2.Implementation["bias"] = 0.0;

                ne3.Implementation["inputgain"] = 1.0;
				ne3.Implementation["outputgain"] = 1.0;
                ne3.Implementation["inputweight"] = 1.0;
                ne3.Implementation["sensorweight"] = 0.0;
                ne3.TimeConst = 10 * BitArrayUtils.ToNDouble(cr.GetGene(5)) + 0.001;
                ne3.Implementation["bias"] = 0.0;

				ne4.Implementation["inputgain"] = 1.0;
				ne4.Implementation["outputgain"] = 2 * BitArrayUtils.ToNDouble(cr.GetGene(6));
				ne4.Implementation["inputweight"] = 1.0;
				ne4.Implementation["sensorweight"] = 0.0;
				ne4.TimeConst = 1.0;
				ne4.Implementation["bias"] = 0.0;

                 net.NumericalMethod["step"] = 4.0 * BitArrayUtils.ToNDouble(cr.GetGene(7)) + 0.05;
                return net;
            };
        }

        public void Run()
        {
			Problem problem = new Problem(genetic, evaluator, stopCondiction, new DebugProblemEventHandler());
			problem.Run ();
			int q = genetic.Population.Count;
			if (q > 0)
			{
				genetic.Population.Sort();

				Genome better = genetic.Population[q-1];
				NeuralNet bnet = (NeuralNet)genetic.Translator(better);
				Console.WriteLine("Synapses: ");
				Console.WriteLine(bnet);
				evaluator.LogEnabled = true;
				Console.WriteLine("FITNESS: {0}", evaluator.evaluate(bnet));
			}   
        }
    }
}
