using System;
using System.Collections.Generic;
using Multinet.Net;
using Multinet.Genetic;
using Multinet.Utils;
using Multinet.Net.Impl;
using Multinet.Net.Ext;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Multinet.Sample
{
	public class MatrixPatternOnlineProblem
	{

        private static Random rnd = new Random();
		public int[][] state;
		public long time;
		public bool training = true;
        public const int INPUT_SIZE = 9,  HIDDEN_SIZE = 20, OUTPUT_SIZE = 1;

        private const float REST = 0.0f;
        private Problem problem;
        private GeneticA genetic;
        private Genome currentGenome;

		public MatrixPatternOnlineProblem ()
		{
            genetic = new GeneticA(200, 0.005);
            genetic.Translator = (Genome gen) => {
                HIRON3 hiron3 = new HIRON3(INPUT_SIZE, HIDDEN_SIZE, OUTPUT_SIZE, gen);
                hiron3.CreateParameters();
                hiron3.CreateNeurons();
                hiron3.CreateSynapses();
                NeuralNet net = hiron3.Net;
                //Console.WriteLine(net);
                return net;
            };
            genetic.GenomeBuilder = () => {
                Genome gen = new Genome();

                Chromossome neuronGain = new Chromossome();
                int n = INPUT_SIZE + HIDDEN_SIZE + OUTPUT_SIZE;
                for (uint i = 0; i < n; i++)
                {
                    neuronGain.AddGene(i, rnd.NextDouble());
                }

                int synapsesNumber = HIDDEN_SIZE * (INPUT_SIZE + OUTPUT_SIZE);
                Chromossome synapses = new Chromossome();
                Chromossome zeroProb = new Chromossome();
                for (uint i = 0; i < synapsesNumber; i++)
                {
                    synapses.AddGene(i, rnd.NextDouble());
                    zeroProb.AddGene(i, rnd.NextDouble());
                }

                gen.AddChromossome(0, neuronGain);
                gen.AddChromossome(1, synapses);
                gen.AddChromossome(2, zeroProb);
                return gen;
            };

            genetic.MinPopulationSize = 200;
            genetic.Elitism = 2;

            problem = new Problem(genetic);
		}

		/// <summary>
		/// Setup this simulation instance.
		/// </summary>
		public void Setup() {
			state = new int[9][];
			state [0] = new int[]{0, 0, 0, 0, 0, 0, 0, 0, 0};
			state [1] = new int[]{0, 1, 0, 
				                  0, 1, 0, 
				                  0, 1, 0};

			state [2] = new int[]{1, 0, 0, 
				                  0, 1, 0, 
				                  0, 0, 1};

			state [3] = new int[]{1, 0, 0, 
				                  1, 0, 0, 
				                  1, 0, 0};

			state [4] = new int[]{0, 0, 1, 
				                  0, 0, 1, 
				                  0, 0, 1};

			state [5] = new int[]{1, 1, 1, 
				                  0, 0, 0, 
				                  0, 0, 0};

			state [6] = new int[]{0, 0, 0, 
				                  1, 1, 1, 
				                  0, 0, 0};

			state [7] = new int[]{0, 0, 0, 
				                  0, 0, 0, 
				                  1, 1, 1};

			state [8] = new int[]{0, 0, 1, 
				                  0, 1, 0, 
				                  1, 0, 0};

			time = 0;
            problem.Start();
            problem.StartEpoch();
            problem.NextGenome(out currentGenome);
		}


		private double sum = 0;
		/// <summary>
		/// Evaluate the specified genome <code>gen<code> based in problem <code>p<code/>.
		/// </summary>
		/// <param name="p">P.</param>
		/// <param name="gen">Gen.</param>
		public void evaluate() {
			currentGenome.Value = System.Math.Max(0.000001f, 10 - sum); //set genome fitness
            genetic.UpdateStatistic(currentGenome.Value, problem);

			sum = 0; //reset to next genome evaluation
		}

        /// <summary>
        /// Gets a random state of current problem instance.
        /// </summary>
        /// <returns>The current state.</returns>
        private int stateidx;
        public int GetCurrentState() {
            int[] nstates = {0, 1, 2, 3, 4, 5, 6, 7, 8};
            int ss = stateidx;
            stateidx++;
            if (stateidx >= nstates.Length)
            {
                stateidx = 0;
            }
            return nstates[ss];
		}
		
		private int live = 0;
        private const int MAX_LIFES = 9;
		/// <summary>
		/// Step this simulation instance.
		/// </summary>
		public bool Step () {
			//Console.WriteLine ("Current Time: {0} ", time);

			if (live >= MAX_LIFES) {
                evaluate();
                if (!problem.NextGenome(out currentGenome))
                {
                    uint surviviors = problem.EndEpoch();
                    Console.WriteLine("Epoch {0} Statistic: MIN = {1}, MAX={2}, AVG={3}", problem.Epoch, problem.Statistic[0], problem.Statistic[1], problem.Statistic[2]/genetic.Population.Count);

                    if (problem.Epoch >= 10)
                    {
                        return false;
                    } else
                    { 
                        problem.StartEpoch();
                        problem.NextGenome(out currentGenome);
                    }

                    if (surviviors < 0)
                    {
                        throw new Exception("Extinção! Ninguém conseguiu evoluir o suficiente!");
                    }
                }
				live = 0;
			}
			NeuralNet net = (NeuralNet)genetic.Translator(currentGenome);
			int s = GetCurrentState ();
            float e = (s == 4 ? 1.0f : 0.0f);
            float c = NetDecision(s, net);
            float erro = e - c;
            sum += erro * erro;
           			
			time++;
			live++;
            return true;
		}

		public void Run(string path="genome.bin") {
			FileStream stream = new FileStream (path, FileMode.Open);
			BinaryFormatter form = new BinaryFormatter ();
			Genome gen = (Genome) form.Deserialize (stream);
			stream.Close();

			NeuralNet net = (NeuralNet) genetic.Translator(gen);

            int userc;
            do
            {
                Console.WriteLine("Select a pattern [0, 8] or -1 to exit: ");
                userc = int.Parse(Console.ReadLine());
                if (userc < 0 || userc > 8)
                {
                    break;
                }

                float c = NetDecision(userc, net);
                Console.WriteLine("NET OUTPUT: {0}", c);
            } while (userc >= 0);
		}

		/// <summary>
		/// Run a simulation instance.
		/// </summary>
		public void Training() {
            
			Setup ();

            while (Step()) ;
			
            List<Genome> pop = genetic.Population;

            pop.Sort();

			Genome last = pop[pop.Count-1];
			last.Serialize ("genome.bin");
		}



		private float NetDecision(int stateIdx, NeuralNet net){

			for (int i = 0; i < INPUT_SIZE; i++) {
				net [i].ProccessInput (state[stateIdx][i] + REST);
			}

			net.Proccess ();
            int t = INPUT_SIZE + HIDDEN_SIZE + OUTPUT_SIZE;

            float outnet = (float)net[t - 1].GetOutput();
            
			return outnet;
		}
	}
}

