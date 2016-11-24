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
            vars["step"] = 0.1;
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

        public double nextState(double runDuration, double currentState, TargetFunction machine)
        {
            double h = this["step"];
            currentState = currentState + h * machine(currentState);
            return currentState;
        }
    }
}
