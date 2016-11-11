using System;
using System.Collections.Generic;
using System.Linq;
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
	public class MatrixPatternDetectionProblem
	{ 

		public int[][] state;
		public long time;
		public bool training = true;
        public const int INPUT_SIZE = 9,  HIDDEN_SIZE = 30, OUTPUT_SIZE = 9;

		private OnlineProblem problem;

		public MatrixPatternDetectionProblem ()
		{


		}

		/// <summary>
		/// Setup this simulation instance.
		/// </summary>
		public void Setup() {
			state = new int[9][];
			state [0] = new int[]{0, 0, 0, 0, 0, 0, 0, 0, 0};
			state [1] = new int[]{0, 1, 0, 
				1, 1, 0, 
				0, 1, 0};

			state [2] = new int[]{1, 1, 0, 
				0, 1, 0, 
				0, 1, 1};

			state [3] = new int[]{1, 1, 1, 
				0, 1, 1, 
				1, 1, 1};

			state [4] = new int[]{1, 0, 1, 
				1, 1, 1, 
				0, 0, 1};

			state [5] = new int[]{1, 1, 1, 
				1, 1, 1, 
				1, 1, 1};

			state [6] = new int[]{1, 0, 0, 
				1, 1, 1, 
				1, 1, 1};

			state [7] = new int[]{1, 1, 1, 
				0, 0, 1, 
				0, 0, 1};

			state [8] = new int[]{1, 1, 1, 
				1, 0, 1, 
				1, 1, 1};

			problem = new OnlineProblem ();



			problem.finalOfEpochHandlerEnd = (OnlineProblem p) => {
				Console.WriteLine("Epoch {0}: MIN_SCORE {1}, MAX_SCORE {2}, AVG {3}", problem.Epoch,
					problem.CurrentMinScore, problem.CurrentMaxScore, problem.CurrentAvgScore);
			};

			problem.startOfEpochHandlerBegin = (OnlineProblem prop) => {

				Console.WriteLine("Starting training in epoch #{0}", prop.Epoch);
			};

			problem.geneticEngine = new GeneticA ();
			problem.geneticEngine.Elitism = 2;
			problem.geneticEngine.MinPopulationSize = 10;
			problem.geneticEngine.PopulationSize = 100;
			problem.geneticEngine.MutationRate = 0.0005;
			problem.geneticEngine.SurvivalRate = 0.9;



			problem.genomeEvaluationHandler = evaluate;



			problem.geneticEngine.Translator = (Genome gen) => {
				HIRON3 hiron3 = new HIRON3(INPUT_SIZE, HIDDEN_SIZE, OUTPUT_SIZE, gen);
				hiron3.CreateNeurons();
				hiron3.CreateSynapses();
				return hiron3.Net;
			};


			problem.geneticEngine.GenomeBuilder = () => {
				Genome gen = new Genome();

				Chromossome neurons = new Chromossome();
				int n = INPUT_SIZE + HIDDEN_SIZE + OUTPUT_SIZE;
				for (uint i = 0; i < n; i++) {
					neurons.AddGene(i, Multinet.Math.PRNG.NextDouble());
				}
				int synapsesNumber = INPUT_SIZE * HIDDEN_SIZE + HIDDEN_SIZE * OUTPUT_SIZE;
				Chromossome synapses = new Chromossome();
				for (uint i = 0; i < synapsesNumber; i++) {
					synapses.AddGene(i, Multinet.Math.PRNG.NextDouble());
				}
				Chromossome numericalStep = new Chromossome();
				numericalStep.AddGene(0, Multinet.Math.PRNG.NextDouble());

				gen.AddChromossome(0, neurons);
				gen.AddChromossome(1, synapses);
				gen.AddChromossome(2, numericalStep);

				return gen;
			};



			time = 0;
			problem.Init ();
			problem.NextGenome ();
		}


		private double sum = 0;
		/// <summary>
		/// Evaluate the specified genome <code>gen<code> based in problem <code>p<code/>.
		/// </summary>
		/// <param name="p">P.</param>
		/// <param name="gen">Gen.</param>
		public double evaluate(OnlineProblem p, Genome gen) {
			gen.Value = 1.0 / (sum + 0.0001f); //set genome fitness
			sum = 0; //reset to next genome evaluation
			return gen.Value;
		}

		/// <summary>
		/// Gets a random state of current problem instance.
		/// </summary>
		/// <returns>The current state.</returns>
		public int GetCurrentState() {
			return (int)(9 * Multinet.Math.PRNG.NextDouble ());
		}
			

		private int live = 0;
		/// <summary>
		/// Step this simulation instance.
		/// </summary>
		public void Step () {
			//Console.WriteLine ("Current Time: {0} ", time);

			if (live > 50) {
				problem.NextGenome ();
				live = 0;
			}
			NeuralNet net = (NeuralNet)problem.CurrentPhenotype;
			int s = GetCurrentState ();
			int c = NetDecision(s, net);

			float diff = System.Math.Abs(s - c);

			sum += diff;

			time++;
			live++;
		}

		public void Run(string path="genome.bin") {
			FileStream stream = new FileStream (path, FileMode.Open);
			BinaryFormatter form = new BinaryFormatter ();
			Genome gen = (Genome) form.Deserialize (stream);
			stream.Close();

			NeuralNet net = (NeuralNet) this.problem.geneticEngine.Translator(gen);


            int userc;
            do
            {
                Console.WriteLine("Select a pattern [0, 8] or -1 to exit: ");
                userc = int.Parse(Console.ReadLine());


                for (int i = 0; i < INPUT_SIZE; i++)
                {
                    net[i].ProccessInput(state[userc][i]);
                }

                net.Proccess();
                int t = INPUT_SIZE + HIDDEN_SIZE + OUTPUT_SIZE;
                int idx = 0;
                float f = float.MinValue;
                int c = idx;
                for (int i = t - 1; i >= (t - 9); i--)
                {
                    float v = (float)net[i].GetOutput();
                    if (v > f)
                    {
                        c = idx;
                        f = v;
                    }
                    idx++;
                }
                Console.WriteLine("NET OUTPUT: {0}", c);
            } while (userc > 0);
		}

		/// <summary>
		/// Run a simulation instance.
		/// </summary>
		public void Training() {
			Setup ();
			for (int i = 0; i < 800000; i++) {
				Step ();
			}
			Genome last = this.problem.CurrentGenome;
			last.Serialize ("genome.bin");
		}



		private int NetDecision(int stateIdx, NeuralNet net){
			for (int i = 0; i < INPUT_SIZE; i++) {
				net [i].ProccessInput (state[stateIdx][i]);
			}

			net.Proccess ();
            int t = INPUT_SIZE + HIDDEN_SIZE + OUTPUT_SIZE;
			int idx = 0;
            float f = float.MinValue;
			int c = idx;
			for (int i = t-1; i >= (t-9); i--) {
				float v = (float) net [i].GetOutput ();
				if (v > f) {
					c = idx;
					f = v;
				}
				idx++;
			}
			return c;
		}
	}
}

