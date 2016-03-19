/**
* This file contains basic implementation of neuralnet.
* For more informations, contact me: gilzamir@outlook.com.
*/
using System.Collections.Generic;
using Multinet.Math;
using Multinet.Genetic;
using System;

namespace Multinet.Net
{
    public class NeuralNet : Net, Evaluable
    {
        private int size;
        private Dictionary<int, Neuron> neurons;
        private Dictionary<Cell, Synapse> synapses;
        private double restInput = 0.0;
        private Dictionary<string, double> vars;
        private double value = 0.0;
        private int nextID = 0;

        public NeuralNet()
        {
            neurons = new Dictionary<int, Neuron>();
            vars = new Dictionary<string, double>();
            synapses = new Dictionary<Cell, Synapse>();
            
        }
        
        public double Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;
            }
        }
        
        public Synapse CreateSynapse(int from, int to, double intensity)
        {
            if (neurons.ContainsKey(from) && neurons.ContainsKey(to))
            {
                Cell pos = new Cell(from, to);
                Neuron ni = neurons[from];
                Neuron nj = neurons[to];

                ni.Outcame.AddLast(pos);
                nj.Incame.AddLast(pos);

                Synapse syn = new Synapse();
                syn.From = from;
                syn.To = to;
                syn.Intensity = intensity;
                synapses.Add(pos, syn);
                return syn;
            } else
            {
                return null;
            }
        }

        public bool RemoveSynapse(Cell pos)
        {
            //TODO implements this method
            return false;
        }

        public int CreateNeuron()
        {
            Neuron ne = new Neuron(nextID);
            neurons[ne.ID] = ne;
            ne.Network = this;
            size++;
            nextID++;
            return ne.ID;
        }
        
        public bool RemoveNeuron(int id)
        {
            if (neurons.Remove(id))
            {
                size--;
                //TODO clean unused synapses
                return true;
            }
            return false;
        }

        public double RestInput
        {
            get
            {
                return restInput;
            }
            set
            {

                restInput = value;
            }
        }

        override
        public double GetDouble(string var)
        {
            return vars[var];
        }

        override
        public void SetDouble(string var, double value)
        {
            vars[var] = value; 
        }

        override
        public int Size
        {
            get
            {
                return size;
            }
        }
        

        override
        public string ToString()
        {
            string txt="";

            foreach (KeyValuePair<Cell, Synapse> pair in synapses)
            {
                txt += string.Format("({0}, {1}) => {2}\n", pair.Key.X, pair.Key.Y, pair.Value.Intensity);
            }

            return txt;
        }

        override
        public Neuron this[int id]
        {
            get
            {
                return neurons[id];
            }

            set
            {
                neurons[id] = value;
            }
        }

        public void Proccess()
        {
            foreach (KeyValuePair<int, Neuron> pair in neurons)
            {
                int id = pair.Key;
                Neuron ne = pair.Value;

                ne.Proccess();
            }
        }

        override
        public Synapse this[int x, int y]
        {
            get
            {
                Cell pos = new Cell(x, y);
                if (synapses.ContainsKey(pos))
                {
                    return synapses[pos];
                } else
                {
                    return null;
                }
            }
        }
    }
}
