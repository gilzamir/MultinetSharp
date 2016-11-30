using System;
using System.Collections.Generic;
using Multinet.Net;
using Multinet.Genetic;
using Multinet.Utils;
using Multinet.Net.Impl;

namespace Multinet.Sample
{
	using rnd = Multinet.Math.PRNG;

    public class PatternEvaluator : Evaluator
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

            double[][] inputs = new double[9][];
            inputs[0] = new double[] { 0, 0, 0,
                                       0, 0, 0,
                                       0, 0, 0};

            inputs[1] = new double[] { 1,  1, 1,
                                       0,  0, 0,
                                       0,  0, 0};

            inputs[2] = new double[] { 1,  0, 0,
                                       0,  1, 0,
                                       0,  0, 1};

            inputs[3] = new double[] { 0,  0, 0,
                                       1,  1, 1,
                                       0,  0, 0};
            inputs[4] = new double[] { 0,  0, 0,
                                       0,  0, 0,
                                       1,  1, 1};

            inputs[5] = new double[] { 0,  0, 1,
                                       0,  1, 0,
                                       1,  0, 0};

            inputs[6] = new double[] { 1,  0, 0,
                                       1,  0, 0,
                                       1,  0, 0};

            inputs[7] = new double[] { 0,  1, 0,
                                       0,  1, 0,
                                       0,  1, 0};

            inputs[8] = new double[] { 0,  0, 1,
                                       0,  0, 1,
                                       0,  0, 1};


            double[] outputs = { 0, 0, 0, 0, 1, 0, 0, 0, 0};

            double error = 0;

            NeuralNet net = (NeuralNet)obj;

            for (uint i = 0; i < inputs.Length; i++)
            {
                double[] input = inputs[i];
                for (uint k = 0; k < 9; k++)
                {
                    net[0].ProccessInput(input[k]);
                }
                net.Proccess();

                double y = net[net.Size-1].GetOutput();


				double e = System.Math.Abs(y-outputs[i]);
                error += e * e;
                

                if (LogEnabled)
                {
                    Console.WriteLine("{0}, {1}, {2} , {3}, {4}, {5}, {6}, {7}, {8}: {9}", 
                        input[0], input[1], input[2], input[3], input[4], input[5], input[6],
                        input[7], input[8], y);
                }
            }
            return  10.0 - error;
        }
    }



    class MatrixPatternOfflineProblem
    {
        GeneticA genetic;
        PatternEvaluator evaluator = new PatternEvaluator();
        uint hiddenSize = 20;
        uint inputSize = 9;
        uint outputSize = 1;

		private static bool stopCondiction(Problem problem){
            return problem.Epoch >= 1000;
		}

        public MatrixPatternOfflineProblem()
        {
            genetic = new GeneticA(500);
            genetic.Elitism = 1;
            genetic.SurvivalRate = 0.1;
            genetic.MutationRate = 0.0005;
            genetic.MinPopulationSize = 500;


            genetic.GenomeBuilder = () =>
            {
                Genome gen = new Genome();
                Chromossome cr = new Chromossome();
                uint numberOfGene = (inputSize+hiddenSize+outputSize) + hiddenSize * (inputSize  + outputSize);
                for (uint i = 0; i < numberOfGene; i++)
                {
                    cr.AddGene(i, rnd.NextDouble());
                }

                Chromossome cr2 = new Chromossome();
                uint numberOfGenes2 = hiddenSize * (inputSize + outputSize);
                for (uint i = 0; i < numberOfGenes2; i++)
                {
                    cr2.AddGene(i, rnd.NextDouble());
                }

                gen.AddChromossome(0, cr);
                gen.AddChromossome(1, cr2);
                return gen;
            };

            genetic.Translator = (Genome gen) =>
            {

                double wamp = 60;
                double wshift = 30;
                NeuralNet net = new NeuralNet();
                Neuron[] ne = new Neuron[inputSize+hiddenSize+outputSize];
                int[] neuronID = new int[ne.Length];
				net.NumericalMethod = new Math.Impl.EulerMethod();
                for (uint i = 0; i < neuronID.Length; i++)
                {
                    neuronID[i] = net.CreateNeuron();
                    ne[i] = net[neuronID[i]];
                    ne[i].Implementation = new Beer1995Neuron();
                }

                Chromossome cr = gen.GetChromossome(0);
                Chromossome cr2 = gen.GetChromossome(1);
                uint idx = 0;
                uint idxSyn = 0;
                double zeroChance = 0.25;
                for (int i = 0; i < inputSize; i++)
                {
                    for (int j = 0; j < hiddenSize; j++)
                    {
                        Synapse syn = net.CreateSynapse(neuronID[i], neuronID[j], wamp * BitArrayUtils.ToNDouble(cr.GetGene(idx++)) - wshift);

                        double p = BitArrayUtils.ToNDouble(cr2.GetGene(idxSyn++));
                        if (p <= zeroChance)
                        {
                            syn.Intensity = 0.0;
                        }
                        
                    }
                }

                for (int i = 0; i < hiddenSize; i++)
                {
                    for (int j = 0; j < outputSize; j++)
                    {
                        Synapse syn = net.CreateSynapse(neuronID[inputSize + i], neuronID[ne.Length-1], wamp * BitArrayUtils.ToNDouble(cr.GetGene(idx++)) - wshift);
                        double p = BitArrayUtils.ToNDouble(cr2.GetGene(idxSyn++));
                        if (p <= zeroChance)
                        {
                            syn.Intensity = 0.0;
                        }
                    }
                }

                for (int i = 0; i < inputSize; i++)
                {
                    ne[i].Implementation.UseNumericalMethod = false;
                    ne[i].Implementation["inputgain"] = 100 * BitArrayUtils.ToNDouble(cr.GetGene(idx++)) - 50;
                    ne[i].Implementation["outputgain"] = 1.0;
                    ne[i].Implementation["inputweight"] = 0.0;
                    ne[i].Implementation["sensorweight"] = 1.0;
                    ne[i].TimeConst = 1.0;
                    ne[i].Implementation["bias"] = 0.0;
                }

                for (int i = 0; i < hiddenSize; i++)
                {
                    ne[9+i].Implementation["inputgain"] = 1.0;
                    ne[9+i].Implementation["outputgain"] = 1.0;
                    ne[9+i].Implementation["inputweight"] = 1.0;
                    ne[9+i].Implementation["sensorweight"] = 0.0;
                    ne[9+i].TimeConst = 4 * BitArrayUtils.ToNDouble(cr.GetGene(idx++)) + 0.001;
                    ne[9+i].Implementation["bias"] = 0.0;
                }

                int outidx = ne.Length - 1;
                ne[outidx].Implementation = new AvgNeuron();
				ne[outidx].Implementation["inputgain"] = 1.0;
				ne[outidx].Implementation["outputgain"] = 2 * BitArrayUtils.ToNDouble(cr.GetGene(idx++));
				ne[outidx].Implementation["inputweight"] = 1.0;
				ne[outidx].Implementation["sensorweight"] = 0.0;
				ne[outidx].TimeConst = 1.0;
				ne[outidx].Implementation["bias"] = 0.0;

                 //net.NumericalMethod["step"] = 1.0 * BitArrayUtils.ToNDouble(cr.GetGene(7)) + 0.000001;
                net.NumericalMethod["step"] = 0.5;
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
