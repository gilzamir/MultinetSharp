using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Multinet.Math;

namespace Multinet.Net
{
    /// <summary>
    /// INeuronImpl is a contract to neuron basic utilities. 
    /// It provides neccessary interface to general network nodes.
    /// </summary>
    public interface INeuronImpl
    {

        /// <summary>
        /// This method returns a function named (<code>funcname</code>).
        /// </summary>
        /// <param name="funcname">Function name</param>
        /// <returns>Function implementation</returns>
        Function GetFunction(string funcname);

        /// <summary>
        /// This method defines a function named <code>funcname</code>.
        /// </summary>
        /// <param name="f">Function</param>
        /// <param name="funcname">Function name</param>
        void SetFunction(Function f, string funcname);

        /// <summary>
        /// This indexer returns a variable value associated with a variable named <code>var</code>. 
        /// </summary>
        /// <param name="var">variable name</param>
        /// <returns>variable value</returns>
        double this[string var]
        {
            get;
            set;
        }

        /// <summary>
        /// This method implements a basic equation that updates neuron state. 
        /// </summary>
        /// <param name="target">Neuron to update</param>
        /// <param name="net">Network with support neuron activity.</param>
        /// <returns></returns>
        double Step(Neuron target, Net net, double state);

        /// <summary>
        /// Returns neuron frequency potential.
        /// </summary>
        /// <param name="n">Target neuron.</param>
        /// <returns>Neuron frequency potential</returns>
        double GetPotential(Neuron n);

        /// <summary>
        /// This method defines a input value of neuron associated with sensor input.
        /// </summary>
        /// <param name="ne">Input neuron</param>
        /// <param name="inv">Sensor value</param>
        /// <returns></returns>
        double SetInput(Neuron ne, double inv);

        /// <summary>
        /// This method returns a neuron output value associated with a network output.
        /// </summary>
        /// <param name="ne"></param>
        /// <returns></returns>
        double GetOutput(Neuron ne);
    }
}
