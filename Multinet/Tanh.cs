using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Multinet.Math
{

    public class Tanh : Function
    {
        public double Exec(double v)
        {
            return System.Math.Tanh(v);
        }

        public double Min()
        {
            return -1.0;
        }

        public double Max()
        {
            return 1.0;
        }
    }

}
