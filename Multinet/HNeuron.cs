using System;
using System.Collections.Generic;
using Multinet.Math;

namespace Multinet.Net.Impl
{
    /// <summary>
    /// Default neuron implementation. 
    /// This neuron implementation added homeostatic behavior to indivudual neuron.
    /// For more informations, see documentation in INeuronImpl interface.
    /// </summary>
    public class HNeuron : ANeuronImpl
    {
        public HNeuron()
        {
            this["bias"] = 0.0;
            this["inputgain"] = 1.0;
            this["outputgain"] = 1.0;
            this["inputweight"] = 1.0;
            this["sensorweight"] = 1.0;
            this["weightgain"] = 1.0;
            this["stateshift"] = -5.0;
            SetFunction(new Multinet.Math.Sigmoid(), "neuronfunction");
            SetFunction(new Multinet.Math.Sin(), "weightfunction");
        }
        
        override
        public double SetInput(Neuron ne, double inv)
        {
            ne.SensorValue = inv * this["inputgain"];
            return ne.SensorValue;
        }

        override
        public double GetPotential(Neuron n)
        {
            return GetFunction("neuronfunction").Exec(n.State + this["bias"]);
        }

        override
        public double GetOutput(Neuron ne)
        {
            return GetPotential(ne) * this["outputgain"];
        }

        override
        public double Step(Neuron target, Net net, double state)
        {
            int nNeurons = net.Size;
            double s = 0;

            foreach (Cell con in target.Incame)
            {
                Synapse syn = net[con.X, con.Y];
                Neuron ne = net[syn.From];
                double zi = ne.Implementation.GetPotential(ne);
                double wij = syn.Intensity;
                s += this["weightgain"] * GetFunction("weightfunction").Exec(wij) * zi; //inputs of neuron id == column id
            }
            double sensorValue = target.SensorValue;
            target.SensorValue = 0;
            double iw = this["inputweight"];
            double sw = this["sensorweight"];
            return (-(state - this["stateshift"]) + s * iw + sensorValue * sw) / target.TimeConst;
        }
    }
}
