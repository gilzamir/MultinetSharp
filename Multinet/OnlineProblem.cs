using System;
using Multinet.Net;
using System.Collections;
using Multinet;
using Multinet.Genetic;
using Multinet.Math;
using Multinet.Utils;
using Multinet.Net.Impl;
using System.Collections.Generic;

namespace Multinet.Genetic
{


	public delegate double GenomeEvaluationHanlder(OnlineProblem problem, Genome cgenome);
	public delegate void EpochResultHandler(OnlineProblem problem, Genome gen);
	public delegate void EpochHandler(OnlineProblem problem);

	public class OnlineProblem
	{
		private long epoch;
		private double[] statistic = { double.MaxValue, double.MinValue, 0.0 };
		private List<Genome> population = null;
		private Genome currentGenome = null;
		private double fitness = 0.0;
		private int currentGenomeIdx=-1;
		private int currentGeneration=-1;
		private Evaluable currentPhenotype = null;

		public GenomeEvaluationHanlder genomeEvaluationHandler = (OnlineProblem p, Genome g) => { return 0.0;};
		public EpochHandler startOfEpochHandlerBegin = (OnlineProblem p) => {};
		public EpochHandler startOfEpochHandlerEnd = (OnlineProblem p) => {};
		public EpochHandler finalOfEpochHandlerBegin = (OnlineProblem p) => {};
		public EpochHandler finalOfEpochHandlerEnd = (OnlineProblem p) => {};
		public EpochResultHandler epochResultHandler = (OnlineProblem p, Genome g) => {};
		public GeneticA geneticEngine = null;



		public OnlineProblem()
		{
		}

		public Genome CurrentGenome {

			get {
				return this.currentGenome;
			}
		}

		public Evaluable CurrentPhenotype {
			get {
				return this.currentPhenotype;
			}
		}

		public double CurrentMinScore {
			get {
				return statistic[0];
			}
		}

		public double CurrentMaxScore{
			get {
				return statistic [1];
			}
		}


		public double CurrentAvgScore {
			get {
				return statistic [2]/population.Count;
			}
		}

		public long Epoch {
			get {
				return epoch;
			}
		}
			
		public void NextGenome() {
			if (currentGenome != null) { //if there is a previous genome...  
				fitness = genomeEvaluationHandler(this, currentGenome);
				geneticEngine.UpdateStatistic (fitness, statistic);
				fitness = 0;
			}

			if (currentGeneration == -1) { //start of training
                startOfEpochHandlerBegin(this);
				population = geneticEngine.Population;
				geneticEngine.InitEvaluation (statistic);
				currentGeneration++;
				startOfEpochHandlerEnd (this);
			}

			currentGenomeIdx++;
			if (currentGenomeIdx >= geneticEngine.PopulationSize) { //end of epoch
				
				finalOfEpochHandlerBegin(this);
				geneticEngine.EndEvaluation (statistic);

				population.Sort ();

				int count = population.Count;

				if (count > 0) { //if there is one or more genomes, than save the better... 
					Genome gen = population [count - 1];
					epochResultHandler (this, gen);
				} else {
					epochResultHandler (this, null);
				}

				//next epoch
				uint gsize = geneticEngine.NextGeneration (statistic);
				if (gsize < 0) {
					throw new Exception ("Population died of starvation. Try again or try other genetic parameters!");
				}
				population = geneticEngine.Population;
				finalOfEpochHandlerEnd(this);

                epoch++;
                startOfEpochHandlerBegin(this);
				geneticEngine.InitEvaluation (statistic);	
				currentGeneration++;
				currentGenomeIdx = 0;
				startOfEpochHandlerEnd (this);
			} 

			currentGenome = population [currentGenomeIdx];
			currentPhenotype = geneticEngine.Translator (currentGenome);
		}


		public void Init() {
			this.geneticEngine.init ();
		}
	}
}

