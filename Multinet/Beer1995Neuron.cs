using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Multinet.Math;

namespace Multinet.Net.Impl
{
    public class Beer1995Neuron : ANeuronImpl
    {
        public Beer1995Neuron()
        {
            this["outputgain"] = 1.0;
            this["inputgain"] = 1.0;
            this["inputweight"] = 1.0;
            this["sensorweight"] = 1.0;
            this["bias"] = 0.0;
            SetFunction(new Multinet.Math.Sigmoid(), "neuronfunction");
        }

        public override double GetOutput(Neuron ne)
        {
            return GetPotential(ne) * this["outputgain"];
        }

        public override double GetPotential(Neuron n)
        {
            return GetFunction("neuronfunction").Exec(n.State + this["bias"]);
        }

        public override double SetInput(Neuron ne, double inv)
        {
            ne.SensorValue = inv * this["inputgain"];
            return ne.SensorValue;
        }

        public override double Step(Neuron target, Net net, double state)
        {
            NeuralNet nnet = (NeuralNet)net;
            int nNeurons = net.Size;
            double s = 0;
            double iw = this["inputweight"];
            double sw = this["sensorweight"];
            if (iw != 0)
            {
                foreach (Cell con in target.Incame)
                {
                    Synapse syn = net[con.X, con.Y];
                    Neuron ne = net[syn.From];
                    double zi = ne.Implementation.GetOutput(ne);
                    double wij = syn.Intensity;
                    //Console.WriteLine("SYNAPSE({0}, {1}) = {2}", con.X, con.Y, wij);
                    s += wij * zi; //inputs of neuron id == column id
                }
            }

            double sensorValue = target.SensorValue;
            target.SensorValue = 0;
            return (-state  + s * iw + sensorValue * sw + nnet.RestInput)/ target.TimeConst;
        }
    }
}
