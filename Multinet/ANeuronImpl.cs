using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Multinet.Math;

namespace Multinet.Net
{
    public abstract class ANeuronImpl : INeuronImpl
    {
        private Dictionary<string, double> doubleVars;
        private Dictionary<String, Function> functions;
        private bool useNumericalMethod;
		private double outputShift, outputAmp;

        public ANeuronImpl()
        {
            useNumericalMethod = true;
            doubleVars = new Dictionary<string, double>();
            functions = new Dictionary<string, Function>();
			outputShift = 0.0;
			outputAmp = 1.0;
        }


		public double OutputShift {
			get {
				return outputShift;
			}

			set {
				outputShift = value;
			}
		}


		public double OutputAmp {
			get {
				return outputAmp;
			} 

			set {
				outputAmp = value;
			}
		}

        public bool UseNumericalMethod
        {
            get
            {
                return this.useNumericalMethod;
            }

            set
            {
                this.useNumericalMethod = value;
            }
        }


        public double this[string var]
        {
            get
            {
                return doubleVars[var];
            }

            set
            {
                doubleVars[var] = value;
            }
        }

        public Function GetFunction(string funcname)
        {
            return functions[funcname];
        }

        public void SetFunction(Function f, string funcname)
        {
            functions[funcname] = f;
        }

        public abstract double GetOutput(Neuron ne);
        public abstract double GetPotential(Neuron n);
        public abstract double SetInput(Neuron ne, double inv);
        public abstract double Step(Neuron target, Net net, double state);
    }
}
