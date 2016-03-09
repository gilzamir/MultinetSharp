using System;
using System.Collections.Generic;
using System.Linq;
using Multinet.Net;
using Multinet.Genetic;
using Multinet.Utils;

namespace Multinet.Sample
{
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

            double[] statistic = { double.MinValue, double.MaxValue, 0.0 };

            double error = 0;

            NeuralNet net = (NeuralNet)obj;

            for (uint i = 0; i < inputs.Length; i++)
            {
                double[] input = inputs[i];
                net[0].ProccessInput(input[0]);
                net[1].ProccessInput(input[1]);
                net.Proccess();
                
                double y = net[2].GetOutput();

                double e = (y-outputs[i]);
                error += e * e;

                if (LogEnabled)
                {
                    Console.WriteLine("{0}, {1} : {2}", input[0], input[1], y);
                }
            }
            return  4.0 - error;
        }
    }

    class XORProblem
    {
        GeneticA genetic;
        XOREvaluator evaluator = new XOREvaluator();

        public XORProblem()
        {
            genetic = new GeneticA(200);
            genetic.Elitism = 0;
            genetic.SurvivalRate = 0.90;
            genetic.MutationRate = 0.00005;
            genetic.MinPopulationSize = 150;

            Random rnd = new Random();

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
                cr.AddGene(8, rnd.NextDouble());

                gen.AddChromossome(0, cr);
                return gen;
            };

            genetic.Translator = (Genome gen) =>
            {
                NeuralNet net = new NeuralNet();


                int n1 = net.CreateNeuron();
                int n2 = net.CreateNeuron();
                int n3 = net.CreateNeuron();

                Neuron ne1 = net[n1];
                Neuron ne2 = net[n2];
                Neuron ne3 = net[n3];

                ne1.Implementation = new Beer1995Neuron();
                ne2.Implementation = new Beer1995Neuron();
                ne3.Implementation = new Beer1995Neuron();

                Chromossome cr = gen.GetChromossome(0);

                net.CreateSynapse(n1, n3, 60 * BitArrayUtils.ToNDouble(cr.GetGene(0))-30);
                net.CreateSynapse(n2, n3, 60 * BitArrayUtils.ToNDouble(cr.GetGene(1))-30);
                
                ne1.Implementation["inputgain"] = 1.0;
                ne1.Implementation["outputgain"] = 1.0;
                ne1.Implementation["inputweight"] = 0.0;
                ne1.Implementation["sensorweight"] = 1.0;
                ne1.TimeConst = 10 * BitArrayUtils.ToNDouble(cr.GetGene(2)) + 0.001;
                ne1.Implementation["bias"] = 200 * BitArrayUtils.ToNDouble(cr.GetGene(3)) - 100;

                ne2.Implementation["inputgain"] = 1.0;
                ne2.Implementation["outputgain"] = 1.0;
                ne2.Implementation["inputweight"] = 0.0;
                ne2.Implementation["sensorweight"] = 1.0;
                ne2.TimeConst = 10 * BitArrayUtils.ToNDouble(cr.GetGene(4)) + 0.001;
                ne2.Implementation["bias"] = 200 * BitArrayUtils.ToNDouble(cr.GetGene(5)) - 100;

                ne3.Implementation["inputgain"] = 1.0;
                ne3.Implementation["outputgain"] = 1.0;
                ne3.Implementation["inputweight"] = 1.0;
                ne3.Implementation["sensorweight"] = 0.0;
                ne3.Implementation["bias"] =  0.0;
                ne3.TimeConst = 10 * BitArrayUtils.ToNDouble(cr.GetGene(6)) + 0.001;
                ne3.Implementation["bias"] = 200 * BitArrayUtils.ToNDouble(cr.GetGene(7)) - 100;

                 net.NumericalMethod["step"] = 4.0 * BitArrayUtils.ToNDouble(cr.GetGene(8)) + 0.05;
               // net.NumericalMethod["step"] = 2.0;
                // Console.WriteLine(net);
                return net;
            };
            genetic.MinPopulationSize = 5;
        }

        public void Run()
        {
            int maxGen = 10;

            double[] statistic = { double.MaxValue, double.MinValue, 0.0 };
            genetic.init();
            for (int i = 0; i < maxGen; i++)
            {
                List<Genome> pop = genetic.Population;
                genetic.InitEvaluation(statistic);
                int n = pop.Count();

                for (int j = 0; j < n; j++)
                {
                    Genome gen = pop.ElementAt(j);

                    //System.Console.WriteLine(gen);
                    gen.Value = genetic.EvaluateOne(gen, evaluator, statistic);
                    //System.Console.WriteLine("GENOME FITNESS: {0}", gen.Value);
                }
                genetic.EndEvaluation(statistic);

                
                Console.WriteLine("GER({0}) :: Fitness(Min, Max, Avg)=({1}, {2}, {3})",
                    i, statistic[0], statistic[1], statistic[2]/pop.Count);


                if (i < maxGen-1)
                {
                    uint survivors = genetic.NextGeneration(statistic);

                    if (survivors <= 0)
                    {
                        Console.WriteLine("População Extinta");
                        break;
                    }
                }
            }

            int q = genetic.Population.Count();
            if (q > 0)
            {
                genetic.Population.Sort();
                Genome better = genetic.Population.ElementAt(q - 1);
                NeuralNet bnet = (NeuralNet)genetic.Translator(better);
                System.Console.WriteLine(bnet);
                evaluator.LogEnabled = true;
                Console.WriteLine("FITNESS: {0}", evaluator.evaluate(bnet));
            }
        }
    }
}
