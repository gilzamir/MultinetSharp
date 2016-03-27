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
			double d = (1 + System.Math.Exp (-v));

			if (d == 0.0) {
				d = 0.00000000000001;
			}

			return 1.0 / d;
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
