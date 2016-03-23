using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Multinet.Math
{
    public interface Function
    {
        double Exec(double v);
        double Min();
        double Max();
    }

}
