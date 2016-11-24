using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Multinet.Net;

namespace Multinet.Genetic
{
    public delegate bool StopCondiction(Problem context);

    public enum EventType
    {
        STOPPED_CONDICTION_SATISFIED,
        POPULATION_EXTINCTION
    }

    public interface ProblemEventHandler
    {
        void handleGaStopped(Problem source, EventType type);
        void handleCurrentStatistic(long popSize, long epoch, double[] statistic);
        void handleCurrentPopulation(long epoch, Problem p, GeneticA genetic);
        void handleEndPopulation(Problem p, GeneticA genetic);
    }


    public class DebugProblemEventHandler : ProblemEventHandler
    {
        public void handleCurrentPopulation(long epoch, Problem p, GeneticA genetic)
        {
            Console.WriteLine("Population at epoch {0} has {1} chromossomes.", epoch, genetic.PopulationSize);
        }

        public void handleCurrentStatistic(long popSize, long epoch, double[] statistic)
        {
            Console.WriteLine("Statistic(MINFITNESS={0}, MAXFITNESS={1}, AVG={2}", statistic[0], statistic[1], statistic[2]/popSize);
        }

        public void handleEndPopulation(Problem p, GeneticA genetic)
        {
            int q = genetic.Population.Count();
            if (q > 0)
            {
                genetic.Population.Sort();

                Genome better = genetic.Population.ElementAt(q - 1);
                Evaluable eval = genetic.Translator(better);
                Console.WriteLine("Evaluable Information: ");
                Console.WriteLine(eval);
                Console.WriteLine("FITNESS: {0}", p.EvaluatorEngine.evaluate(eval));
            }
        }

        public void handleGaStopped(Problem source, EventType type)
        {
            if (type == EventType.POPULATION_EXTINCTION)
            {
                Console.WriteLine("Population Extinction!");
            } else if (type == EventType.STOPPED_CONDICTION_SATISFIED)
            {
                Console.WriteLine("End of Training!");
            }
        }
    }

    public class Problem
    {
        private GeneticA geneticEngine;
        private Evaluator evaluatorEngine;
        private StopCondiction stopCondiction;
        private ProblemEventHandler eventHandler;
		private long epoch;
        private double[] statistic;

		public long Epoch {
			get {
				return epoch;
			}
		}

        public Evaluator EvaluatorEngine
        {
            get
            {
                return evaluatorEngine;
            }
        }

        public Problem(GeneticA genetic, Evaluator evaluator, StopCondiction method, ProblemEventHandler eventHandler)
        {
            this.evaluatorEngine = evaluator;
            this.geneticEngine = genetic;
            this.stopCondiction = method;
            this.eventHandler = eventHandler;
        }


        public Problem(GeneticA genetic)
        {
            this.evaluatorEngine = null;
            this.geneticEngine = genetic;
            this.stopCondiction = null;
            this.eventHandler = null;
        }

        public void Start()
        {
            statistic = new double[]{ double.MaxValue, double.MinValue, 0.0 };
            geneticEngine.init();


            epoch = 0;
        }

        private int currentGenome = 0;
        
        public void StartEpoch()
        {
            geneticEngine.InitEvaluation(statistic);
            currentGenome = 0;
        }


        public bool NextGenome(out Genome next)
        {
            if (currentGenome < geneticEngine.Population.Count) {
                next = geneticEngine.Population.ElementAt(currentGenome++);
                return true;
            } else {
                next = null;
                return false;
            }
        }

        public uint EndEpoch()
        {
            geneticEngine.EndEvaluation(statistic);
            epoch++;
            return geneticEngine.NextGeneration(statistic); //surviviors
        }
        
        public double[] Statistic
        {
            get
            {
                return statistic;
            }

        }

        public void Run()
        {
            statistic = new double[] { double.MaxValue, double.MinValue, 0.0 };
            geneticEngine.init();


            epoch = 0;
            while (true)
            {
                List<Genome> pop = geneticEngine.Population;
                geneticEngine.InitEvaluation(statistic);
                int n = pop.Count();

                for (int j = 0; j < n; j++)
                {
                    Genome gen = pop.ElementAt(j);
                    gen.Value = geneticEngine.EvaluateOne(gen, evaluatorEngine, statistic);
                }

                geneticEngine.EndEvaluation(statistic);

                eventHandler.handleCurrentStatistic(n, epoch, statistic);

                epoch++;


                if (this.stopCondiction(this))
                {
                    eventHandler.handleGaStopped(this, EventType.STOPPED_CONDICTION_SATISFIED);
                    break;
                }
                else
                {
                    uint survivors = geneticEngine.NextGeneration(statistic);

                    if (survivors <= 0)
                    {
                        eventHandler.handleGaStopped(this, EventType.POPULATION_EXTINCTION);
                        break;
                    }
                }
                eventHandler.handleCurrentPopulation(epoch-1, this, this.geneticEngine);
            }
            eventHandler.handleEndPopulation(this, this.geneticEngine);
        }

    }
}
