using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Multinet.Math
{
    public class IdentityFunction : Function
    {
        public double Exec(double v)
        {
            return v;
        }

        public double Min()
        {
            return double.MinValue;
        }

        public double Max()
        {
            return double.MaxValue;
        }
    }
}
