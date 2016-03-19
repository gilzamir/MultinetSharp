using System;
using System.Collections.Generic;
using System.Linq;

namespace Multinet.Genetic
{
    public class GeneticA
    {
        private List<Genome> genomes;
        private MutationMethod mutationMethod;
        private CrossoverMethod crossoverMethod;
        private GenomeBuilderMethod builderMethod;
        private GenomeTranslatorMethod translatorMethod;
        private uint populationSize;
        private double mutationRate;
        private uint numberOfEvaluations;
        private double survivalRate;
        private uint minPopulationSize;
        private bool selectDuplicatedGenome;
        private int elitism;
        private Random rnd = new Random();

        public GeneticA(uint psize = 100, double mr = 0.05)
        {
            this.populationSize = psize;
            this.mutationRate = mr;
            this.survivalRate = 1.0;
            this.numberOfEvaluations = 0;
            this.minPopulationSize = 0;
            this.elitism = 2;
            this.selectDuplicatedGenome = false;
            InitializeDefaultBehavior();
        }

        public int Elitism
        {
            get
            {
                return elitism;
            }

            set
            {
                elitism = value;
            }
        }


        public bool SelectDuplicatedGenome
        {
            get
            {
                return selectDuplicatedGenome;
            }

            set
            {
                selectDuplicatedGenome = value;
            }
        }

        public uint MinPopulationSize
        {
            get
            {
                return minPopulationSize;
            }

            set
            {
                minPopulationSize = value;
            }
        }

        public double SurvivalRate
        {
            get
            {
                return survivalRate;
            }

            set
            {
                survivalRate = value;
            }
        }

        public List<Genome> Population
        {
            get
            {
                return genomes;
            }
        }

        public GenomeTranslatorMethod Translator
        {
            get
            {
                return this.translatorMethod;
            }

            set
            {
                this.translatorMethod = value;
            }
        }

        public uint PopulationSize
        {
            get
            {
                return populationSize;
            }

            set
            {
                this.populationSize = value;
            }
        }

        public double MutationRate
        {
            get
            {
                return this.mutationRate;
            }

            set
            {
                this.mutationRate = value;
            }
        }

        public GenomeBuilderMethod GenomeBuilder
        {
            get
            {
                return this.builderMethod;
            }

            set
            {
                this.builderMethod = value;
            }
        }


        public CrossoverMethod Crossover
        {
            get
            {
                return this.crossoverMethod;
            }

            set
            {
                this.crossoverMethod = value;
            }
        }

        public MutationMethod Mutation
        {
            get
            {
                return this.mutationMethod;
            }

            set
            {
                mutationMethod = value;
            }
        }

        public void init()
        {
            CheckPrerequistes();
            genomes = new List<Genome>();
            numberOfEvaluations = 0;
            for (uint i = 0; i < populationSize; i++)
            {
                genomes.Add(GenomeBuilder());
            }
        }

        public void InitEvaluation(double[] statistic)
        {
            statistic[0] = double.MaxValue;
            statistic[1] = double.MinValue;
            statistic[2] = 0.0;
            numberOfEvaluations = 0;
        }

        public void EndEvaluation(double[] statistic)
        {

        }

        public double EvaluateOne(Genome genome, Evaluator evaluator, double[] statistic)
        {
            if (statistic == null || statistic.Length < 3)
            {
                throw new ArgumentException("Invalid array of statistic information.");
            }

            Evaluable evaluable = Translator(genome);
            double fitness = evaluator.evaluate(evaluable);
            statistic[2] += fitness;
            this.numberOfEvaluations++;
            if (fitness < statistic[0])
            {
                statistic[0] = fitness;
            }

            if (fitness > statistic[1])
            {
                statistic[1] = fitness;
            }

            return fitness;
        }

        public uint NextGeneration(double[] statistic)
        {
            List<Genome> parents = new List<Genome>();
            List<Genome> children = new List<Genome>();
            int n = genomes.Count();

            genomes.Sort();

            if (elitism > 0)
            {
                int added = 0;
                for (int i = n - 1; i >= 0 && added < elitism; i--)
                {
                    children.Add(genomes[i]);
                    added++;
                }
            }

            int survivors = (int)(populationSize * survivalRate);

            Random roulet = new Random();

            Dictionary<int, int> selected = new Dictionary<int, int>();
            //Console.WriteLine("SURVIVORS: {0}", survivors);

            int q = 0;
            while (q < survivors)
            {
                double pos = roulet.NextDouble();
                double region = 0;
                for (int i = 0; i < n; i++)
                {
                    Genome current = genomes[i];
                    region += (current.Value / statistic[2]);

                    if (pos <= region && !selected.ContainsKey(i))
                    {
                        selected[i] = i;
                        parents.Add(current);
                        q++;
                        break;
                    } 
                }
                //System.Console.WriteLine("COMP {0}      {1}", position, region);
                
            }


            parents.Sort();
            n = parents.Count();
          //  System.Console.WriteLine("PARENTS SIZE: {0}", n);
            for (int i = n - 1; i >= 0; i--)
            {
                Genome g1 = parents.ElementAt(i);
                for (int j = n-1; j >= 0; j--)
                {
                    if (i != j)
                    {
                        Genome g2 = parents.ElementAt(j);
                        Genome child1 = crossoverMethod(g1, g2);
                        
                        if (child1 != null)
                        {
                            mutationMethod(child1, MutationRate);
                            children.Add(child1);
                        }           
                    }
                }
            }

            uint m = (uint)children.Count();
            if (m < minPopulationSize)
            {
                uint r = minPopulationSize - (uint)children.Count();
                for (uint k = 0; k < r; k++)
                {
                    children.Add(builderMethod());
                }
            }

            genomes = children;
            return (uint)genomes.Count();
        }


        private void CheckPrerequistes()
        {
            if (builderMethod == null)
            {
                throw new GenomeBuilderNotDefinedException();
            }

            if (translatorMethod == null)
            {
                throw new GenomeTranslatorNotDefinedException();
            }
        }

        private void InitializeDefaultBehavior()
        {
            this.builderMethod = null;
            this.crossoverMethod = CrossoverImpl.CrossoverMethod;
            this.mutationMethod = MutationImpl.MutationMethod;
        }
    }
}
