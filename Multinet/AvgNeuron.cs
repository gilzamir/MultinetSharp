using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Multinet.Math;


namespace Multinet.Net.Impl
{
    public class AvgNeuron : ANeuronImpl
    {
        public AvgNeuron()
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
            return (OutputAmp * GetPotential(ne) - OutputShift) * this["outputgain"];
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
            int count = 0;
            double iw = this["inputweight"];
            if (iw != 0)
            {
                foreach (Cell con in target.Incame)
                {
                    Synapse syn = net[con.X, con.Y];
                    Neuron ne = net[syn.From];
                    double zi = ne.Implementation.GetPotential(ne);
                    double wij = syn.Intensity;
                    s += wij * zi; //inputs of neuron id == column id
                    count++;
                }
            }

            double nstate = s / count;

            if (nstate < -100)
            {
                nstate = -100;
            }
            else if (nstate > 100)
            {
                nstate = 100;
            }
            return nstate;
        }
    }
}
