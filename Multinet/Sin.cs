using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multinet.Math
{
    public class Sin : Function
    {
        public double Exec(double v)
        {
            return System.Math.Sin(v);
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
