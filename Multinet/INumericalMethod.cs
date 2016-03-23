using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Multinet.Math
{
    public interface INumericalMethod
    {
        double nextState(double currentState, TargetFunction machine);
        double this[string var]
        {
            get;
            set;
        }
    }
}
