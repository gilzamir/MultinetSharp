﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Multinet.Math;

namespace Multinet.Net
{
    public abstract class ANeuronImpl : INeuronImpl
    {
        Dictionary<string, double> doubleVars;
        private Dictionary<String, Function> functions;

        public ANeuronImpl()
        {
            doubleVars = new Dictionary<string, double>();
            functions = new Dictionary<string, Function>();
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