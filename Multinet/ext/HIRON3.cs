using System;
using Multinet.Net;
using System.Collections;
using Multinet;
using Multinet.Genetic;
using Multinet.Math;
using Multinet.Utils;
using Multinet.Net.Impl;
using System.Collections.Generic;

namespace Multinet.Net.Ext
{
	
	public enum NeuronType {
		INPUT,
		HIDDEN,
		OUTPUT
	}

    public delegate void CreateParametersHandler(HIRON3 net);
	public delegate void ConfigureNeuronHandler(HIRON3 net, int id, NeuronType type);
	public delegate void ConfigureSynapseHandler(HIRON3 net, Synapse syn, NeuronType fromType, NeuronType toType);

	/// <summary>
	/// HIRO3 is HIdden Recursive Operation is a network with three layers and recurive hidden layer.
	/// In this network type, input and output layers are not recursive.
	/// </summary>
	public class HIRON3
	{
		private NeuralNet net;

		private int[] input = null;
		private int[] hidden = null;
		private int[] output = null;
		private int neurons = 0, synapses = 0;
		private Genome genotype = null;

		public HIRON3 (int inputSize, int hiddenSize, int outputSize, Genome genotype = null)
		{
			input = new int[inputSize];
			output = new int[outputSize];
			hidden = new int[hiddenSize];
			neurons = inputSize + hiddenSize + outputSize;
			synapses =  hiddenSize * (inputSize +  outputSize);
			this.genotype = genotype;
            chrNeuronIdx = 0;
            chrSynIdx = 0;
		}

		private static uint chrNeuronIdx = 0;

        public CreateParametersHandler createParametersHandler = (HIRON3 hiron) => {
            //hiron.net.NumericalMethod = new Math.Impl.EulerMethod();
            //hiron.net.NumericalMethod["step"] = 0.5;
        };
        
		public ConfigureNeuronHandler configureNeuronHandler = (HIRON3 hiron, int id, NeuronType type) => {
			double gain;
            
			Genome genotype = hiron.genotype;
			Neuron ne = hiron.Net[id];
			Chromossome chrGain = genotype.GetChromossome(0);
            

            if (type == NeuronType.INPUT) {

				if (genotype != null) {
					gain = 100 * BitArrayUtils.ToNDouble(chrGain.GetGene(chrNeuronIdx)) - 50;
                } else {
					gain = 100 * Math.PRNG.NextDouble() - 50;
                }

                ne.Implementation = new Beer1995NeuronStateless();
				ne.Implementation.UseNumericalMethod = false;
				ne.Implementation["inputgain"] = gain;
				ne.Implementation["outputgain"] = 1.0;
				ne.Implementation["inputweight"] = 0.0;
				ne.Implementation["sensorweight"] = 1.0;
				ne.TimeConst = 1.0;
				ne.Implementation["bias"] = 0.0;
			} 

			if (type == NeuronType.HIDDEN) {

				if (genotype != null) {
					gain = 10 * BitArrayUtils.ToNDouble(chrGain.GetGene(chrNeuronIdx)) - 5;
                } else {
					gain = 10 * Math.PRNG.NextDouble() - 5;
        		}
                ne.Implementation = new Beer1995NeuronStateless();
                ne.Implementation.UseNumericalMethod = false;
				ne.Implementation["inputgain"] = 1.0;
				ne.Implementation["outputgain"] = 1.0;
				ne.Implementation["inputweight"] = 1.0;
				ne.Implementation["sensorweight"] = 0.0;
				ne.TimeConst = 1.0;
				ne.Implementation["bias"] = gain;
			}

			if (type == NeuronType.OUTPUT) {
                if (genotype != null)
                {
                    gain = 2 * BitArrayUtils.ToNDouble(chrGain.GetGene(chrNeuronIdx)) + 0.001;
                }
                else
                {
                    gain = 2 * Math.PRNG.NextDouble() + 0.001;
                }

                ne.Implementation = new AvgNeuron();
                ne.Implementation.UseNumericalMethod = false;
                ne.Implementation["inputgain"] = 1.0;
				ne.Implementation["outputgain"] = gain;
				ne.Implementation["inputweight"] = 1.0;
				ne.Implementation["sensorweight"] = 0.0;
				ne.TimeConst = 1.0;
				ne.Implementation["bias"] = 0.0;
			}
            chrNeuronIdx++;
			if (chrNeuronIdx >= hiron.neurons) {
				chrNeuronIdx = 0;
			}
		};

        public Genome Genotype
        {
            get
            {
                return genotype;
            }
        }

            
		private static uint chrSynIdx = 0;
		public ConfigureSynapseHandler ConfigureSynapseHandler = (HIRON3 hiron, Synapse syn, 
			NeuronType fromType, NeuronType toType) => {
			    Chromossome chr = hiron.genotype.GetChromossome(1);
                Chromossome chr2 = hiron.genotype.GetChromossome(2);
                BitArray gene = chr.GetGene(chrSynIdx);
                BitArray gene2 = chr2.GetGene(chrSynIdx++);

                double chance = BitArrayUtils.ToNDouble(gene2);

                if (chance >= 0.25)
                {
                    syn.Intensity = 60 * BitArrayUtils.ToNDouble(gene) - 30;
                } else
                {
                    syn.Intensity = 0;
                }

                if (chrSynIdx >= hiron.synapses) {
                    chrSynIdx = 0;
                }
		};

		public NeuralNet Net {
			get {
				return this.net;
			}
		}

        public void CreateParameters()
        {
            net = new NeuralNet();
            createParametersHandler(this);
        }

        public void CreateNeurons() {
			for (int i = 0; i < input.Length; i++) {
				input[i] = net.CreateNeuron();
				this.configureNeuronHandler (this, input[i], NeuronType.INPUT);
			}

			for (int i = 0; i < hidden.Length; i++) {
                hidden [i] = net.CreateNeuron ();
                this.configureNeuronHandler (this, hidden [i], NeuronType.HIDDEN);
			}


			for (int i = 0; i < output.Length; i++) {
				output [i] = net.CreateNeuron ();
                this.configureNeuronHandler (this, output [i], NeuronType.OUTPUT);
			}
		}

		public void CreateSynapses() {
			for (int i = 0; i < input.Length; i++) {
				for (int j = 0; j < hidden.Length; j++) {
					Synapse syn = net.CreateSynapse (input[i], hidden[j], 0);
					this.ConfigureSynapseHandler (this, syn, NeuronType.INPUT, NeuronType.HIDDEN);
				}
			}
            
            for (int i = 0; i < hidden.Length; i++)
            {
                for (int j = 0; j < output.Length; j++)
                {
                    Synapse syn = net.CreateSynapse(hidden[i], output[j], 0);
                    this.ConfigureSynapseHandler(this, syn, NeuronType.HIDDEN, NeuronType.OUTPUT);
                }
            }
        }

        public int CountNeurons
        {
            get
            {
                return neurons;
            }
        }

        public int CountSynapses
        {
            get
            {
                return this.synapses;
            }
        }
	}
}

