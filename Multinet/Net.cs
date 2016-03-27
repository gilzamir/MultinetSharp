using Multinet.Math;
using Multinet.Math.Impl;


namespace Multinet.Net
{
    public abstract class Net
    {
        private INumericalMethod method;

        public Net()
        {
            method = new EulerMethod();
        }

        public INumericalMethod NumericalMethod
        {
            get
            {
                return method;
            }

            set
            {
                method = value;
            }
        }

        public abstract Synapse this[int i, int j]
        {
            get;
        }

        public abstract int Size
        {
            get;
        }

        public abstract double GetDouble(string var);
        public abstract void SetDouble(string var, double v);

        public abstract Neuron this[int idx]
        {
            get;
            set;
        }

    }
}