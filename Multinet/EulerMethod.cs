using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Multinet.Math.Impl
{
    public class EulerMethod : INumericalMethod
    {
        private Dictionary<string, double> vars;

        public EulerMethod()
        {
            vars = new Dictionary<string, double>();
            vars["step"] = 0.5;
        }

        public double this[string var]
        {
            get
            {
                return vars[var];
            }

            set
            {
                vars[var] = value;
            }
        }

        public double nextState(double currentState, TargetFunction machine)
        {
            return currentState + this["step"] * machine(currentState);
        }
    }
}
