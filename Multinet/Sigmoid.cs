using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Multinet.Math
{
    public class Sigmoid : Function
    {
        public double Exec(double v)
        {
            return 1.0 / (1 + System.Math.Exp(-v));
        }

        public double Min()
        {
            return 0.0;
        }

        public double Max()
        {
            return 1.0;
        }
    }
}
